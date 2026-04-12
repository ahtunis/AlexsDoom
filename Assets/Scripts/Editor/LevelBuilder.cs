#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using AlexsDoom.Player;
using AlexsDoom.Level;
using AlexsDoom.Enemies;
using AlexsDoom.Weapons;
using AlexsDoom.Pickups;
using AlexsDoom.UI;

namespace AlexsDoom.Editor
{
    /// <summary>
    /// Menu: AlexsDoom ▶ Build Level01
    ///
    /// Generates a complete, playable Level01 scene from scratch.
    /// Run once after opening the project — re-running rebuilds from scratch.
    ///
    /// Level layout (top-down, Z+ = north):
    ///
    ///  Z=80  ╔══════════════════════╗
    ///        ║   ●  (Exit trigger)  ║   ← DemonEnemy guards this zone
    ///  Z=58  ║  ██             ██  ║
    ///        ║                      ║
    ///  Z=40  ║  ██  ██ [DOOR] ██  ║   ← LockedDoor (needs Red KeyCard)
    ///        ║                      ║
    ///  Z=20  ║  ██  K        A ██  ║   ← K=KeyCard  A=Armor  H=Health
    ///        ║       H              ║
    ///  Z=0   ╚══════════════════════╝   ← Player spawns here
    ///         X=0                X=24
    /// </summary>
    public static class LevelBuilder
    {
        // ── Shared assets ─────────────────────────────────────────────────────────
        private static Material _floorMat, _wallMat, _doorMat, _pillarMat;
        private static TMP_FontAsset _font;

        // ── Entry point ───────────────────────────────────────────────────────────
        [MenuItem("AlexsDoom/Build Level01 %#l")]
        public static void BuildLevel01()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            PrepareAssets();
            SetupLighting();

            var geoRoot    = NewEmpty("--- Geometry ---");
            var actorRoot  = NewEmpty("--- Actors ---");
            var pickupRoot = NewEmpty("--- Pickups ---");

            BuildArena(geoRoot);

            var projPrefab = BuildProjectilePrefab();
            var (player, camRoot, weaponHolder, cam) = BuildPlayer();
            player.transform.SetParent(actorRoot.transform);
            BuildEnemies(actorRoot, projPrefab);
            BuildPickups(pickupRoot);

            var hud = BuildHUD(player, cam);
            WirePlayer(player, hud, weaponHolder);
            BuildLevelExit(hud);
            BuildManagers();

            UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

            const string path = "Assets/Scenes/Level01.unity";
            EditorSceneManager.SaveScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene(), path);

            Debug.Log($"[AlexsDoom] Level01 saved → {path}");
        }

        // ── Assets ────────────────────────────────────────────────────────────────
        private static void PrepareAssets()
        {
            _floorMat  = Mat("FloorMat",  new Color(0.22f, 0.20f, 0.17f));
            _wallMat   = Mat("WallMat",   new Color(0.28f, 0.26f, 0.23f));
            _doorMat   = Mat("DoorMat",   new Color(0.55f, 0.15f, 0.10f));
            _pillarMat = Mat("PillarMat", new Color(0.32f, 0.30f, 0.27f));

            _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Packages/com.unity.textmeshpro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        }

        private static Material Mat(string n, Color c)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var m = new Material(shader) { name = n };
            m.color = c;
            return m;
        }

        // ── Lighting ──────────────────────────────────────────────────────────────
        private static void SetupLighting()
        {
            RenderSettings.ambientLight     = new Color(0.12f, 0.10f, 0.09f);
            RenderSettings.ambientIntensity = 1f;

            var sunGo = new GameObject("Sun");
            sunGo.transform.eulerAngles = new Vector3(50f, -30f, 0f);
            var light = sunGo.AddComponent<Light>();
            light.type      = LightType.Directional;
            light.intensity = 1.1f;
            light.color     = new Color(1f, 0.93f, 0.82f);
        }

        // ── Geometry ──────────────────────────────────────────────────────────────
        private static void BuildArena(GameObject root)
        {
            const float W  = 24f;          // arena width (X)
            const float L  = 80f;          // arena length (Z)
            const float H  = 5f;           // wall height
            const float T  = 0.4f;         // wall thickness
            const float DW = 4f;           // locked-door gap width
            const float midZ = L * 0.5f;   // Z=40, mid-wall

            // Floor
            NavStatic(Box(root, "Floor",
                new Vector3(W/2, -0.1f, L/2), new Vector3(W, 0.2f, L), _floorMat));

            // Ceiling
            Box(root, "Ceiling",
                new Vector3(W/2, H + 0.1f, L/2), new Vector3(W, 0.2f, L), _wallMat);

            // Perimeter walls
            NavStatic(Box(root, "Wall_S", new Vector3(W/2, H/2, 0),   new Vector3(W, H, T), _wallMat));
            NavStatic(Box(root, "Wall_N", new Vector3(W/2, H/2, L),   new Vector3(W, H, T), _wallMat));
            NavStatic(Box(root, "Wall_W", new Vector3(0,   H/2, L/2), new Vector3(T, H, L), _wallMat));
            NavStatic(Box(root, "Wall_E", new Vector3(W,   H/2, L/2), new Vector3(T, H, L), _wallMat));

            // Mid-wall — gap centred at X = W/2 for the locked door
            float gL = W/2 - DW/2;  // 10
            float gR = W/2 + DW/2;  // 14
            NavStatic(Box(root, "MidWall_W",
                new Vector3(gL/2,         H/2, midZ), new Vector3(gL,     H, T), _wallMat));
            NavStatic(Box(root, "MidWall_E",
                new Vector3((gR + W)/2,   H/2, midZ), new Vector3(W - gR, H, T), _wallMat));

            // Locked door in gap
            var doorGo = Box(root, "LockedDoor",
                new Vector3(W/2, H/2, midZ), new Vector3(DW, H, T), _doorMat);
            doorGo.AddComponent<AudioSource>().playOnAwake = false;
            doorGo.AddComponent<LockedDoor>();

            // Cover pillars — two columns, four rows
            float[] pz = { 16f, 26f, 54f, 64f };
            float[] px = { 5f, 19f };
            foreach (float z in pz)
                foreach (float x in px)
                    NavStatic(Box(root, $"Pillar_{(int)x}_{(int)z}",
                        new Vector3(x, H/2, z), new Vector3(3f, H, 3f), _pillarMat));
        }

        // ── EnemyProjectile prefab ────────────────────────────────────────────────
        private static EnemyProjectile BuildProjectilePrefab()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "EnemyProjectile";
            go.transform.localScale = Vector3.one * 0.3f;
            go.GetComponent<Renderer>().material = Mat("ProjMat", new Color(1f, 0.4f, 0f));
            var col = go.GetComponent<SphereCollider>();
            col.isTrigger = true;
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            go.AddComponent<EnemyProjectile>();

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/EnemyProjectile.prefab");
            Object.DestroyImmediate(go);
            return prefab.GetComponent<EnemyProjectile>();
        }

        // ── Player ────────────────────────────────────────────────────────────────
        private static (GameObject player, GameObject camRoot, GameObject weaponHolder, Camera cam)
            BuildPlayer()
        {
            var player = NewEmpty("Player", new Vector3(12f, 0f, 4f));
            player.tag = "Player";

            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.4f;
            cc.center = new Vector3(0f, 0.9f, 0f);

            player.AddComponent<PlayerHealth>();
            player.AddComponent<KeyInventory>();
            player.AddComponent<DeathHandler>();  // HUD wired later

            var footAudio = player.AddComponent<AudioSource>();
            footAudio.playOnAwake  = false;
            footAudio.spatialBlend = 0f;
            player.AddComponent<FootstepAudio>();

            // Camera root at eye level
            var camRoot = NewEmpty("CameraRoot", new Vector3(0f, 1.7f, 0f));
            camRoot.transform.SetParent(player.transform, false);

            var camGo = NewEmpty("MainCamera");
            camGo.transform.SetParent(camRoot.transform, false);
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.fieldOfView   = 75f;
            cam.nearClipPlane = 0.08f;
            camGo.AddComponent<AudioListener>();

            // Weapon holder (child of camera root)
            var wh = NewEmpty("WeaponHolder", new Vector3(0.25f, -0.28f, 0.55f));
            wh.transform.SetParent(camRoot.transform, false);

            // Pistol
            var pistolGo = NewEmpty("Pistol");
            pistolGo.transform.SetParent(wh.transform, false);
            var pAudio = pistolGo.AddComponent<AudioSource>();
            pAudio.playOnAwake = false; pAudio.spatialBlend = 0f;
            var pistol = pistolGo.AddComponent<Pistol>();
            Set(pistol, "weaponName", "Pistol");
            Set(pistol, "damage",    12);
            Set(pistol, "fireRate",  0.35f);
            Set(pistol, "maxAmmo",   50);
            Set(pistol, "playerCamera", cam);

            // Shotgun (inactive at start)
            var shotgunGo = NewEmpty("Shotgun");
            shotgunGo.transform.SetParent(wh.transform, false);
            var sAudio = shotgunGo.AddComponent<AudioSource>();
            sAudio.playOnAwake = false; sAudio.spatialBlend = 0f;
            var shotgun = shotgunGo.AddComponent<Shotgun>();
            Set(shotgun, "weaponName", "Shotgun");
            Set(shotgun, "damage",     18);
            Set(shotgun, "fireRate",   0.85f);
            Set(shotgun, "maxAmmo",    20);
            Set(shotgun, "playerCamera", cam);
            shotgunGo.SetActive(false);

            // PlayerController
            var pc = player.AddComponent<PlayerController>();
            Set(pc, "cameraRoot",       camRoot.transform);
            Set(pc, "moveSpeed",        8f);
            Set(pc, "mouseSensitivity", 2f);

            // WeaponBob
            var wb = player.AddComponent<WeaponBob>();
            Set(wb, "weaponHolder", wh.transform);

            return (player, camRoot, wh, cam);
        }

        private static void WirePlayer(GameObject player, HUDController hud, GameObject weaponHolder)
        {
            // Weapons array
            var pistol  = weaponHolder.transform.Find("Pistol")?.GetComponent<WeaponBase>();
            var shotgun = weaponHolder.transform.Find("Shotgun")?.GetComponent<WeaponBase>();
            var handler = player.AddComponent<WeaponHandler>();
            SetArray(handler, "weapons", new Object[] { pistol, shotgun });
            Set(handler, "hud", hud);

            // Death handler
            Set(player.GetComponent<DeathHandler>(), "hud", hud);
        }

        // ── Enemies ───────────────────────────────────────────────────────────────
        private static void BuildEnemies(GameObject root, EnemyProjectile projPrefab)
        {
            // South half (before locked door)
            Enemy<BasicEnemy>(root, "BasicEnemy_1", new Vector3( 6f, 0f, 15f),
                maxHp:50,  dmg:10, atkR:2f,  detR:20f);
            Enemy<BasicEnemy>(root, "BasicEnemy_2", new Vector3(18f, 0f, 25f),
                maxHp:50,  dmg:10, atkR:2f,  detR:20f);
            Enemy<ImpEnemy>(  root, "Imp_1",         new Vector3( 8f, 0f, 32f),
                maxHp:30,  dmg:8,  atkR:2f,  detR:25f);

            // North half (past locked door)
            var rangedGo = Enemy<RangedEnemy>(root, "RangedEnemy_1", new Vector3(12f, 0f, 52f),
                maxHp:60,  dmg:10, atkR:18f, detR:30f);
            var firePoint = NewEmpty("FirePoint", new Vector3(0f, 1f, 0.6f));
            firePoint.transform.SetParent(rangedGo.transform, false);
            var re = rangedGo.GetComponent<RangedEnemy>();
            Set(re, "projectilePrefab", projPrefab);
            Set(re, "firePoint",        firePoint.transform);
            Set(re, "preferredRange",   10f);

            Enemy<DemonEnemy>(root, "Demon_1", new Vector3(12f, 0f, 72f),
                maxHp:200, dmg:25, atkR:3f,  detR:22f);
        }

        private static GameObject Enemy<T>(GameObject root, string n, Vector3 pos,
            int maxHp, int dmg, float atkR, float detR) where T : EnemyBase
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = n;
            go.transform.SetParent(root.transform);
            go.transform.position = pos;
            go.GetComponent<Renderer>().material = Mat(n + "Mat", new Color(0.65f, 0.18f, 0.10f));

            var agent = go.AddComponent<NavMeshAgent>();
            agent.height = 2f;
            agent.radius = 0.4f;
            agent.speed  = 3.5f;

            var e = go.AddComponent<T>();
            Set(e, "maxHealth",      maxHp);
            Set(e, "damage",         dmg);
            Set(e, "attackRange",    atkR);
            Set(e, "detectionRange", detR);
            Set(e, "attackCooldown", 1.5f);
            return go;
        }

        // ── Pickups ───────────────────────────────────────────────────────────────
        private static void BuildPickups(GameObject root)
        {
            Pickup<HealthPickup>(root, "HealthPickup_1", new Vector3( 3f, 0.5f,  8f),
                Color.green,  "healAmount",  25);
            Pickup<AmmoPickup>(  root, "AmmoPickup_1",   new Vector3(21f, 0.5f, 14f),
                Color.yellow, "ammoAmount",  15);

            // Red KeyCard (needed for locked door)
            var keyGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            keyGo.name = "KeyCard_Red";
            keyGo.transform.SetParent(root.transform);
            keyGo.transform.position     = new Vector3(3f, 0.5f, 36f);
            keyGo.transform.localScale   = new Vector3(0.3f, 0.5f, 0.1f);
            keyGo.GetComponent<Renderer>().material = Mat("KeyCardMat", Color.red);
            keyGo.GetComponent<Collider>().isTrigger = true;
            keyGo.AddComponent<KeyCard>();  // defaults to Red (first enum value)

            Pickup<ArmorPickup>( root, "ArmorPickup_1",  new Vector3(21f, 0.5f, 50f),
                Color.cyan,   "armorAmount", 50);
            Pickup<HealthPickup>(root, "HealthPickup_2", new Vector3(12f, 0.5f, 68f),
                Color.green,  "healAmount",  50);
        }

        private static void Pickup<T>(GameObject root, string n, Vector3 pos,
            Color col, string field, int val) where T : Pickup
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = n;
            go.transform.SetParent(root.transform);
            go.transform.position   = pos;
            go.transform.localScale = Vector3.one * 0.5f;
            go.GetComponent<Renderer>().material = Mat(n + "Mat", col);
            go.GetComponent<Collider>().isTrigger = true;
            var p = go.AddComponent<T>();
            Set(p, field, val);
        }

        // ── HUD ───────────────────────────────────────────────────────────────────
        private static HUDController BuildHUD(GameObject player, Camera cam)
        {
            var canvasGo = NewEmpty("HUD_Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode    = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder  = 0;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Stats bar (bottom strip)
            var bar = Panel(canvasGo, "StatsBar",
                new Vector2(0,0), new Vector2(1,0), new Vector2(0,0), new Vector2(0,50),
                new Color(0,0,0,0.7f));
            var healthTxt = Txt(bar, "HealthText", "HP: 100", new Vector2(0,    0), new Vector2(0.25f,1));
            var armorTxt  = Txt(bar, "ArmorText",  "AR: 0",   new Vector2(0.25f,0), new Vector2(0.50f,1));
            var ammoTxt   = Txt(bar, "AmmoText",   "50 / 50", new Vector2(0.50f,0), new Vector2(0.75f,1));
            var killTxt   = Txt(bar, "KillText",   "Kills: 0",new Vector2(0.75f,0), new Vector2(1.00f,1));

            // Full-screen damage flash
            var flashGo = Panel(canvasGo, "DamageFlash",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Color.clear);
            flashGo.AddComponent<DamageFlash>();

            // Game-over panel
            var goPanel = Panel(canvasGo, "GameOverPanel",
                new Vector2(0.25f, 0.35f), new Vector2(0.75f, 0.65f),
                Vector2.zero, Vector2.zero, new Color(0,0,0,0.88f));
            Txt(goPanel, "GOTitle", "YOU DIED", Vector2.zero, Vector2.one, 52, Color.red);
            goPanel.SetActive(false);

            // Pause panel
            var pausePanel = Panel(canvasGo, "PausePanel",
                new Vector2(0.3f,0.2f), new Vector2(0.7f,0.8f),
                Vector2.zero, Vector2.zero, new Color(0,0,0,0.88f));
            Txt(pausePanel, "PauseTitle", "PAUSED", new Vector2(0,0.8f), Vector2.one, 40, Color.white);
            pausePanel.SetActive(false);

            // Level-end panel
            var endPanel = Panel(canvasGo, "LevelEndPanel",
                new Vector2(0.2f,0.2f), new Vector2(0.8f,0.8f),
                Vector2.zero, Vector2.zero, new Color(0,0,0,0.92f));
            Txt(endPanel, "EndTitle",  "LEVEL COMPLETE", new Vector2(0,0.7f), new Vector2(1,1),  44, Color.yellow);
            var killEndTxt = Txt(endPanel, "EndKills", "Kills: 0",     new Vector2(0,0.5f), new Vector2(1,0.7f));
            var timeEndTxt = Txt(endPanel, "EndTime",  "Time: 00:00",  new Vector2(0,0.3f), new Vector2(1,0.5f));
            endPanel.SetActive(false);

            var levelEnd = endPanel.AddComponent<LevelEndPanel>();
            Set(levelEnd, "panel",         endPanel);
            Set(levelEnd, "killCountText", killEndTxt);
            Set(levelEnd, "timeText",      timeEndTxt);

            var hud = canvasGo.AddComponent<HUDController>();
            Set(hud, "healthText",    healthTxt);
            Set(hud, "armorText",     armorTxt);
            Set(hud, "ammoText",      ammoTxt);
            Set(hud, "killCountText", killTxt);
            Set(hud, "gameOverPanel", goPanel);

            var pm = canvasGo.AddComponent<PauseMenu>();
            Set(pm, "pausePanel", pausePanel);

            return hud;
        }

        // ── Level exit ────────────────────────────────────────────────────────────
        private static void BuildLevelExit(HUDController hud)
        {
            var go = NewEmpty("LevelExit", new Vector3(12f, 1.5f, 78f));
            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(8f, 3f, 2f);

            var exit = go.AddComponent<LevelExit>();
            Set(exit, "nextSceneName",  "MainMenu");
            var lep = hud.GetComponentInChildren<LevelEndPanel>(true);
            Set(exit, "levelEndPanel",  lep);

            // Green exit marker
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "ExitMarker";
            marker.transform.SetParent(go.transform);
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localScale = new Vector3(2f, 4f, 2f);
            marker.GetComponent<Renderer>().material = Mat("ExitMat", new Color(0.1f, 1f, 0.25f));
            Object.DestroyImmediate(marker.GetComponent<Collider>());
        }

        // ── Managers ──────────────────────────────────────────────────────────────
        private static void BuildManagers()
        {
            var gm = NewEmpty("GameManager").AddComponent<GameManager>();
            Set(gm, "firstLevelScene", "Level01");
            Set(gm, "mainMenuScene",   "MainMenu");

            var amGo = NewEmpty("AudioManager");
            amGo.AddComponent<AudioSource>().playOnAwake = false;
            amGo.AddComponent<AudioManager>();
        }

        // ── UI helpers ────────────────────────────────────────────────────────────
        private static GameObject Panel(GameObject parent, string n,
            Vector2 aMin, Vector2 aMax, Vector2 oMin, Vector2 oMax, Color col)
        {
            var go  = NewEmpty(n);
            go.transform.SetParent(parent.transform, false);
            var img = go.AddComponent<Image>();
            img.color = col;
            var rt  = go.GetComponent<RectTransform>();
            rt.anchorMin = aMin; rt.anchorMax = aMax;
            rt.offsetMin = oMin; rt.offsetMax = oMax;
            return go;
        }

        private static TextMeshProUGUI Txt(GameObject parent, string n, string text,
            Vector2 aMin, Vector2 aMax, int size = 22, Color? col = null)
        {
            var go = NewEmpty(n);
            go.transform.SetParent(parent.transform, false);
            var t  = go.AddComponent<TextMeshProUGUI>();
            t.text      = text;
            t.fontSize  = size;
            t.color     = col ?? Color.white;
            t.alignment = TextAlignmentOptions.Center;
            if (_font != null) t.font = _font;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = aMin; rt.anchorMax = aMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            return t;
        }

        // ── Geometry helpers ──────────────────────────────────────────────────────
        private static GameObject Box(GameObject parent, string n,
            Vector3 pos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = n;
            go.transform.SetParent(parent.transform);
            go.transform.position   = pos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().material = mat;
            return go;
        }

        private static GameObject NavStatic(GameObject go)
        {
            GameObjectUtility.SetStaticEditorFlags(go,
                StaticEditorFlags.NavigationStatic | StaticEditorFlags.ContributeGI);
            return go;
        }

        // ── General helpers ───────────────────────────────────────────────────────
        private static GameObject NewEmpty(string n, Vector3 pos = default)
        {
            var go = new GameObject(n);
            go.transform.position = pos;
            return go;
        }

        private static void Set(Object target, string field, object value)
        {
            var so   = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null)
            {
                Debug.LogWarning($"[LevelBuilder] '{field}' not found on {target.GetType().Name}");
                return;
            }
            switch (value)
            {
                case int    v: prop.intValue              = v; break;
                case float  v: prop.floatValue            = v; break;
                case bool   v: prop.boolValue             = v; break;
                case string v: prop.stringValue           = v; break;
                case Color  v: prop.colorValue            = v; break;
                case Vector3 v: prop.vector3Value         = v; break;
                case Object  v: prop.objectReferenceValue = v; break;
                default:
                    Debug.LogWarning($"[LevelBuilder] Unsupported type {value.GetType()} for '{field}'");
                    return;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetArray(Object target, string field, Object[] items)
        {
            var so   = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null) return;
            prop.arraySize = items.Length;
            for (int i = 0; i < items.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
