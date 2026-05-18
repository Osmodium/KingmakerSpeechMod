using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeechMod.Unity.Extensions;

public static class AssetIdFinder
{
    /// <summary>
    /// Main entry point: tries multiple strategies to find where "ButtonEdge" lives.
    /// </summary>
    public static void FindPrefabContainingChild(string childName)
    {
        Main.Logger.Log($"=== AssetIdFinder: Searching for '{childName}' ===");

        // Strategy 1: Search all loaded GameObjects (scenes + prefabs in memory)
        FindInLoadedGameObjects(childName);

        // Strategy 2: Dump scene info for scenes containing the child
        FindInLoadedScenes(childName);

        // Strategy 3: Reflect into ResourcesLibrary's internal cache/dictionary
        FindInResourcesLibraryCache(childName);

        // Strategy 4: Scan loaded AssetBundles
        FindInLoadedAssetBundles(childName);

        Main.Logger.Log($"=== AssetIdFinder: Search complete ===");
    }

    /// <summary>
    /// Strategy 1: Use Resources.FindObjectsOfTypeAll to find all GameObjects with the target name.
    /// This finds objects in scenes AND loaded-but-not-instantiated prefabs.
    /// </summary>
    private static void FindInLoadedGameObjects(string childName)
    {
        Main.Logger.Log("[Strategy 1] Searching all loaded GameObjects...");

        var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        Main.Logger.Log($"  Total GameObjects in memory: {allGameObjects.Length}");

        var matches = allGameObjects.Where(go => go.name == childName).ToList();
        Main.Logger.Log($"  Found {matches.Count} GameObject(s) named '{childName}'");

        foreach (var match in matches)
        {
            var path = GetPath(match.transform);
            var scene = match.scene;
            var isSceneObject = scene.IsValid();
            var hideFlags = match.hideFlags;

            Main.Logger.Log("  --------------------------------------------------");
            Main.Logger.Log($"  Name: {match.name}");
            Main.Logger.Log($"  Full path: {path}");
            Main.Logger.Log($"  HideFlags: {hideFlags}");

            if (isSceneObject)
            {
                Main.Logger.Log($"  Scene: {scene.name} (path: {scene.path})");
            }
            else
            {
                Main.Logger.Log("  NOT in a scene (likely a prefab/asset in memory)");
                // Try to find the root and log its info
                var root = match.transform.root.gameObject;
                Main.Logger.Log($"  Root object: {root.name} (InstanceID: {root.GetInstanceID()})");
            }

            Main.Logger.Log("  --------------------------------------------------");
        }
    }

    /// <summary>
    /// Strategy 2: Search each loaded scene's hierarchy for the child.
    /// </summary>
    private static void FindInLoadedScenes(string childName)
    {
        Main.Logger.Log("[Strategy 2] Searching loaded scenes...");

        var sceneCount = SceneManager.sceneCount;
        Main.Logger.Log($"  Loaded scenes: {sceneCount}");

        for (var i = 0; i < sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded)
                continue;

            Main.Logger.Log($"  Checking scene: {scene.name} (path: {scene.path})");

            foreach (var root in scene.GetRootGameObjects())
            {
                var found = FindChildRecursive(root.transform, childName);
                if (found != null)
                {
                    Main.Logger.Log($"  >>> FOUND in scene '{scene.name}' at path: {GetPath(found)}");
                }
            }
        }
    }

    /// <summary>
    /// Strategy 3: Reflect into ResourcesLibrary to find its internal loaded resources cache
    /// and check if any loaded prefab contains the child.
    /// </summary>
    private static void FindInResourcesLibraryCache(string childName)
    {
        Main.Logger.Log("[Strategy 3] Reflecting into ResourcesLibrary internals...");

        var resourcesLibraryType = FindResourcesLibraryType();
        if (resourcesLibraryType == null)
        {
            Main.Logger.Log("  Could not find ResourcesLibrary type.");
            return;
        }

        Main.Logger.Log($"  Found type: {resourcesLibraryType.FullName}");

        // Log all static fields and properties for discovery
        var allFields = resourcesLibraryType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Main.Logger.Log($"  Static fields ({allFields.Length}):");
        foreach (var field in allFields)
        {
            Main.Logger.Log($"    {field.FieldType.Name} {field.Name}");
        }

        var allProps = resourcesLibraryType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Main.Logger.Log($"  Static properties ({allProps.Length}):");
        foreach (var prop in allProps)
        {
            Main.Logger.Log($"    {prop.PropertyType.Name} {prop.Name}");
        }

        // Try to find dictionary fields that map string -> object (asset caches)
        foreach (var field in allFields)
        {
            object value;
            try { value = field.GetValue(null); }
            catch { continue; }

            if (value == null) continue;

            // Check if it's a dictionary
            if (value is IDictionary dict)
            {
                Main.Logger.Log($"  Scanning dictionary field '{field.Name}' ({dict.Count} entries)...");
                ScanDictionaryForPrefab(dict, childName);
            }
            // Check nested object's fields for dictionaries (go one level deeper)
            else if (!value.GetType().IsValueType && !value.GetType().IsPrimitive)
            {
                ScanObjectForDictionaries(value, field.Name, childName, 0);
            }
        }
    }

    private static void ScanObjectForDictionaries(object obj, string parentName, string childName, int depth)
    {
        if (obj == null || depth > 3) return;

        var type = obj.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            object value;
            try { value = field.GetValue(obj); }
            catch { continue; }

            if (value == null) continue;

            if (value is IDictionary dict)
            {
                if (dict.Count > 0 && dict.Count < 100000)
                {
                    Main.Logger.Log($"  Scanning nested dict '{parentName}.{field.Name}' ({dict.Count} entries)...");
                    ScanDictionaryForPrefab(dict, childName);
                }
            }
            else if (!value.GetType().IsValueType && !value.GetType().IsPrimitive && !(value is string))
            {
                ScanObjectForDictionaries(value, $"{parentName}.{field.Name}", childName, depth + 1);
            }
        }
    }

    private static void ScanDictionaryForPrefab(IDictionary dict, string childName)
    {
        var count = 0;
        foreach (DictionaryEntry entry in dict)
        {
            if (count++ > 10000) break; // Safety limit

            var key = entry.Key?.ToString() ?? "(null)";

            // Check if value is or contains a GameObject
            GameObject go = null;
            if (entry.Value is GameObject directGo)
            {
                go = directGo;
            }
            else if (entry.Value is UnityEngine.Object uObj)
            {
                go = uObj as GameObject;
            }
            // Try to get GameObject from wrapper objects via reflection
            else if (entry.Value != null)
            {
                var resourceField = entry.Value.GetType().GetField("Resource", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var resource = resourceField?.GetValue(entry.Value);
                go = resource as GameObject;

                if (go == null)
                {
                    var objField = entry.Value.GetType().GetField("Object", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var objValue = objField?.GetValue(entry.Value);
                    go = objValue as GameObject;
                }
            }

            if (go == null) continue;

            var found = FindChildRecursive(go.transform, childName);
            if (found != null)
            {
                Main.Logger.Log("  ==================================================");
                Main.Logger.Log($"  FOUND PREFAB IN CACHE!");
                Main.Logger.Log($"  Key/AssetId: {key}");
                Main.Logger.Log($"  Prefab name: {go.name}");
                Main.Logger.Log($"  Child path: {GetPath(found)}");
                Main.Logger.Log("  ==================================================");
            }
        }
    }

    /// <summary>
    /// Strategy 4: Check all loaded AssetBundles for prefabs containing the child.
    /// </summary>
    private static void FindInLoadedAssetBundles(string childName)
    {
        Main.Logger.Log("[Strategy 4] Scanning loaded AssetBundles...");

        var allBundles = AssetBundle.GetAllLoadedAssetBundles().ToList();
        Main.Logger.Log($"  Loaded bundles: {allBundles.Count}");

        foreach (var bundle in allBundles)
        {
            if (bundle == null) continue;

            Main.Logger.Log($"  Checking bundle: {bundle.name}");

            string[] assetNames;
            try { assetNames = bundle.GetAllAssetNames(); }
            catch { continue; }

            // Only check assets that look like they could be UI prefabs
            var prefabAssets = assetNames
                .Where(n => n.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) ||
                            n.IndexOf("dialog", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            n.IndexOf("ui", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            n.IndexOf("canvas", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (prefabAssets.Count == 0)
            {
                // If no obvious UI prefabs, try all assets but limit
                prefabAssets = assetNames.Take(200).ToList();
            }

            foreach (var assetName in prefabAssets)
            {
                GameObject prefab;
                try { prefab = bundle.LoadAsset<GameObject>(assetName); }
                catch { continue; }

                if (prefab == null) continue;

                var found = FindChildRecursive(prefab.transform, childName);
                if (found != null)
                {
                    Main.Logger.Log("  ==================================================");
                    Main.Logger.Log($"  FOUND IN ASSET BUNDLE!");
                    Main.Logger.Log($"  Bundle: {bundle.name}");
                    Main.Logger.Log($"  Asset name: {assetName}");
                    Main.Logger.Log($"  Prefab name: {prefab.name}");
                    Main.Logger.Log($"  Child path: {GetPath(found)}");
                    Main.Logger.Log("  ==================================================");
                }
            }
        }
    }

    /// <summary>
    /// Iterates ResourceNamesByAssetId, loads each as GameObject, and checks if it contains the child.
    /// This finds the loadable parent asset that contains ButtonEdge.
    /// </summary>
    public static void FindAssetContainingChild(string childName)
    {
        Main.Logger.Log($"=== FindAssetContainingChild: Searching for '{childName}' ===");

        var resourcesLibraryType = FindResourcesLibraryType();
        if (resourcesLibraryType == null)
        {
            Main.Logger.Log("  Could not find ResourcesLibrary type.");
            return;
        }

        // Get the TryGetResource<T>(string) method
        var allMethods = resourcesLibraryType
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(m => m.Name == "TryGetResource")
            .ToList();

        Main.Logger.Log($"  Found {allMethods.Count} TryGetResource method(s):");
        foreach (var m in allMethods)
        {
            var parms = string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            Main.Logger.Log($"    {m.Name}<{(m.IsGenericMethodDefinition ? "T" : "")}> ({parms}) - IsGeneric={m.IsGenericMethodDefinition}");
        }

        var tryGetResourceMethod = allMethods
            .FirstOrDefault(m => m.IsGenericMethodDefinition && m.GetParameters().Length >= 1);

        if (tryGetResourceMethod == null)
        {
            Main.Logger.Log("  Could not find a suitable TryGetResource method.");
            return;
        }

        Main.Logger.Log($"  Using method with {tryGetResourceMethod.GetParameters().Length} param(s).");
        var tryGetGameObject = tryGetResourceMethod.MakeGenericMethod(typeof(GameObject));

        // Get ResourceNamesByAssetId dictionary
        var libObject = resourcesLibraryType
            .GetProperty("LibraryObject", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?
            .GetValue(null);

        if (libObject == null)
        {
            // Try via field
            libObject = resourcesLibraryType
                .GetField("s_LibraryObject", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?
                .GetValue(null);
        }

        if (libObject == null)
        {
            Main.Logger.Log("  Could not get LibraryObject.");
            return;
        }

        // Get ResourceNamesByAssetId
        var resourceNamesField = libObject.GetType()
            .GetField("<ResourceNamesByAssetId>k__BackingField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (resourceNamesField == null)
        {
            // Try property
            var prop = libObject.GetType().GetProperty("ResourceNamesByAssetId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                var dictObj = prop.GetValue(libObject);
                if (dictObj is IDictionary dict)
                {
                    Main.Logger.Log($"  Found ResourceNamesByAssetId via property ({dict.Count} entries).");
                    SearchDictForAsset(dict, tryGetGameObject, childName);
                    return;
                }
            }
            Main.Logger.Log("  Could not find ResourceNamesByAssetId field or property.");
            return;
        }

        var resourceNamesDict = resourceNamesField.GetValue(libObject) as IDictionary;
        if (resourceNamesDict == null)
        {
            Main.Logger.Log("  ResourceNamesByAssetId is null.");
            return;
        }

        Main.Logger.Log($"  Found ResourceNamesByAssetId ({resourceNamesDict.Count} entries).");
        SearchDictForAsset(resourceNamesDict, tryGetGameObject, childName);
    }

    private static void SearchDictForAsset(IDictionary resourceNamesDict, MethodInfo tryGetGameObject, string childName)
    {
        // Determine how many params the method needs
        var paramCount = tryGetGameObject.GetParameters().Length;
        Main.Logger.Log($"  TryGetResource param count: {paramCount}");

        // First log some entries to understand the format
        var sampleCount = 0;
        foreach (DictionaryEntry entry in resourceNamesDict)
        {
            if (sampleCount++ < 10)
                Main.Logger.Log($"  Sample: key={entry.Key}, value={entry.Value}");
        }

        // Filter for entries whose resource name suggests UI content
        var candidates = new List<string>();
        foreach (DictionaryEntry entry in resourceNamesDict)
        {
            var assetId = entry.Key?.ToString();
            var resourceName = entry.Value?.ToString() ?? "";

            if (string.IsNullOrEmpty(assetId))
                continue;

            // Try all entries - filter by name containing UI/dialog/canvas related terms
            var nameLower = resourceName.ToLowerInvariant();
            if (nameLower.IndexOf("ui", StringComparison.Ordinal) >= 0 ||
                nameLower.IndexOf("dialog", StringComparison.Ordinal) >= 0 ||
                nameLower.IndexOf("canvas", StringComparison.Ordinal) >= 0 ||
                nameLower.IndexOf("button", StringComparison.Ordinal) >= 0 ||
                nameLower.IndexOf("scroll", StringComparison.Ordinal) >= 0 ||
                nameLower.IndexOf("prefab", StringComparison.Ordinal) >= 0)
            {
                candidates.Add(assetId);
            }
        }

        Main.Logger.Log($"  Filtered to {candidates.Count} UI-related candidates. Trying to load each...");

        var matchCount = 0;
        foreach (var assetId in candidates)
        {
            GameObject prefab;
            try
            {
                prefab = InvokeGetResource(tryGetGameObject, assetId, paramCount);
            }
            catch
            {
                continue;
            }

            if (prefab == null)
                continue;

            var found = FindChildRecursive(prefab.transform, childName);
            if (found != null)
            {
                matchCount++;
                Main.Logger.Log("  ==================================================");
                Main.Logger.Log($"  FOUND! Asset contains '{childName}'!");
                Main.Logger.Log($"  AssetId: {assetId}");
                Main.Logger.Log($"  Prefab name: {prefab.name}");
                Main.Logger.Log($"  Child path: {GetPath(found)}");
                Main.Logger.Log("  ==================================================");
            }
        }

        if (matchCount == 0)
        {
            Main.Logger.Log("  No UI candidates contained the child. Trying ALL entries...");

            // Try everything
            foreach (DictionaryEntry entry in resourceNamesDict)
            {
                var assetId = entry.Key?.ToString();
                if (string.IsNullOrEmpty(assetId))
                    continue;

                GameObject prefab;
                try
                {
                    prefab = InvokeGetResource(tryGetGameObject, assetId, paramCount);
                }
                catch
                {
                    continue;
                }

                if (prefab == null)
                    continue;

                var found = FindChildRecursive(prefab.transform, childName);
                if (found != null)
                {
                    matchCount++;
                    Main.Logger.Log("  ==================================================");
                    Main.Logger.Log($"  FOUND! Asset contains '{childName}'!");
                    Main.Logger.Log($"  AssetId: {assetId}");
                    Main.Logger.Log($"  Resource name: {entry.Value}");
                    Main.Logger.Log($"  Prefab name: {prefab.name}");
                    Main.Logger.Log($"  Child path: {GetPath(found)}");
                    Main.Logger.Log("  ==================================================");
                }
            }
        }

        Main.Logger.Log($"  Done. Found {matchCount} asset(s) containing '{childName}'.");
    }

    private static GameObject InvokeGetResource(MethodInfo method, string assetId, int paramCount)
    {
        // Build args array with defaults for extra params
        var args = new object[paramCount];
        args[0] = assetId;
        for (var i = 1; i < paramCount; i++)
        {
            var paramType = method.GetParameters()[i].ParameterType;
            args[i] = paramType.IsValueType ? System.Activator.CreateInstance(paramType) : null;
        }
        return method.Invoke(null, args) as GameObject;
    }

    /// <summary>
    /// Finds which asset bundle contains a given scene, and logs all scene-containing bundles.
    /// Also checks if the scene can be loaded via SceneManager.
    /// </summary>
    public static void FindSceneBundle(string sceneName)
    {
        Main.Logger.Log($"=== FindSceneBundle: Looking for '{sceneName}' ===");

        // 1. Check all loaded asset bundles for scene paths
        var allBundles = AssetBundle.GetAllLoadedAssetBundles().ToList();
        Main.Logger.Log($"  Loaded bundles: {allBundles.Count}");

        var foundBundles = new List<string>();

        foreach (var bundle in allBundles)
        {
            if (bundle == null) continue;

            string[] scenePaths;
            try { scenePaths = bundle.GetAllScenePaths(); }
            catch { continue; }

            if (scenePaths == null || scenePaths.Length == 0)
                continue;

            Main.Logger.Log($"  Bundle '{bundle.name}' contains {scenePaths.Length} scene(s):");
            foreach (var path in scenePaths)
            {
                Main.Logger.Log($"    {path}");
                if (path.IndexOf(sceneName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foundBundles.Add(bundle.name);
                    Main.Logger.Log($"    >>> MATCH! Bundle '{bundle.name}' contains '{sceneName}'");
                }
            }
        }

        if (foundBundles.Count == 0)
        {
            Main.Logger.Log($"  No loaded bundle contains scene '{sceneName}'.");
            Main.Logger.Log("  Checking if scene is in build settings or loadable...");

            // Try to see if it's loadable
            var sceneCount = SceneManager.sceneCountInBuildSettings;
            Main.Logger.Log($"  Scenes in build settings: {sceneCount}");
            for (var i = 0; i < sceneCount; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                Main.Logger.Log($"    [{i}] {path}");
                if (path.IndexOf(sceneName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Main.Logger.Log($"    >>> MATCH at build index {i}!");
                }
            }
        }

        // 2. Also dump all bundle names that have ANY assets (to help identify naming patterns)
        Main.Logger.Log("  --- All bundles with scene content or 'ui' in name: ---");
        foreach (var bundle in allBundles)
        {
            if (bundle == null) continue;
            var name = bundle.name ?? "";
            if (name.IndexOf("ui", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("scene", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("ingame", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Main.Logger.Log($"    Bundle: {name}");
                try
                {
                    var scenes = bundle.GetAllScenePaths();
                    if (scenes.Length > 0)
                        Main.Logger.Log($"      Scenes: {string.Join(", ", scenes)}");
                    var assets = bundle.GetAllAssetNames();
                    Main.Logger.Log($"      Assets: {assets.Length} total");
                    foreach (var a in assets.Take(10))
                        Main.Logger.Log($"        {a}");
                }
                catch { }
            }
        }

        Main.Logger.Log($"=== FindSceneBundle complete ===");
    }

    private static Type FindResourcesLibraryType()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(SafeGetTypes)
            .FirstOrDefault(t =>
                t.Name == "ResourcesLibrary" &&
                t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .Any(m => m.Name == "TryGetResource"));
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch { return Array.Empty<Type>(); }
    }

    private static Transform FindChildRecursive(Transform root, string childName)
    {
        if (root == null) return null;
        if (root.name == childName) return root;

        for (var i = 0; i < root.childCount; i++)
        {
            var result = FindChildRecursive(root.GetChild(i), childName);
            if (result != null) return result;
        }

        return null;
    }

    private static string GetPath(Transform transform)
    {
        var path = transform.name;

        while (transform.parent != null)
        {
            transform = transform.parent;
            path = $"{transform.name}/{path}";
        }

        return path;
    }

    /// <summary>
    /// Dumps all sprite/texture info from a named GameObject and its children.
    /// Call this when the UI_Ingame_Scene is loaded to discover sprite names.
    /// </summary>
    public static void DumpButtonEdgeVisuals(string gameObjectPath)
    {
        Main.Logger.Log($"=== DumpButtonEdgeVisuals: '{gameObjectPath}' ===");

        var transform = GameObject.Find(gameObjectPath)?.transform;
        if (transform == null)
        {
            Main.Logger.Log("  GameObject not found!");
            return;
        }

        DumpVisualsRecursive(transform, 0);

        Main.Logger.Log("=== DumpButtonEdgeVisuals complete ===");
    }

    private static void DumpVisualsRecursive(Transform t, int depth)
    {
        var indent = new string(' ', depth * 2);
        Main.Logger.Log($"{indent}GameObject: {t.name} (active: {t.gameObject.activeSelf})");

        var rectTransform = t.GetComponent<RectTransform>();
        if (rectTransform != null)
            Main.Logger.Log($"{indent}  RectTransform: sizeDelta={rectTransform.sizeDelta}, anchoredPos={rectTransform.anchoredPosition}");

        var images = t.GetComponents<UnityEngine.UI.Image>();
        foreach (var img in images)
        {
            Main.Logger.Log($"{indent}  Image: sprite={img.sprite?.name ?? "null"}, texture={img.sprite?.texture?.name ?? "null"}, color={img.color}, type={img.type}");
            if (img.sprite != null)
                Main.Logger.Log($"{indent}    Sprite rect={img.sprite.rect}, pivot={img.sprite.pivot}, border={img.sprite.border}");
        }

        var rawImages = t.GetComponents<UnityEngine.UI.RawImage>();
        foreach (var raw in rawImages)
        {
            Main.Logger.Log($"{indent}  RawImage: texture={raw.texture?.name ?? "null"}, color={raw.color}");
        }

        // Dump all components for full picture
        var components = t.GetComponents<Component>();
        Main.Logger.Log($"{indent}  Components: {string.Join(", ", components.Select(c => c?.GetType().Name ?? "null"))}");

        for (var i = 0; i < t.childCount; i++)
            DumpVisualsRecursive(t.GetChild(i), depth + 1);
    }
}