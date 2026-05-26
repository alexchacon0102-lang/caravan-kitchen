// ============================================================
// CARAVAN KITCHEN — OrderManager.cs
// Script #11 de 14
// PROGRESO: Economy/OrderManager.cs ✅ completado
// SIGUIENTE: Economy/CurrencyManager.cs
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CaravanKitchen.Core;
using CaravanKitchen.Cooking;

namespace CaravanKitchen.Economy
{
    public enum OrderType    { Normal, Premium, Rare, Event }
    public enum OrderStatus  { Pending, Completed, Expired }

    [System.Serializable]
    public class OrderData
    {
        public string      orderId;
        public string      clientName;
        public string      recipeId;
        public string      dishName;
        public int         reward;
        public int         fameReward;
        public OrderType   type         = OrderType.Normal;
        public OrderStatus status       = OrderStatus.Pending;
        public float       timeLimit    = 120f;   // segundos (0 = sin limite)
        public float       timeElapsed  = 0f;
        public Sprite      clientSprite;
    }

    /// <summary>
    /// Gestiona los pedidos activos de la caravana.
    /// Genera pedidos automáticamente basados en recetas desbloqueadas.
    /// </summary>
    public class OrderManager : MonoBehaviour
    {
        public static OrderManager Instance { get; private set; }

        [Header("Config de pedidos")]
        [SerializeField] private int   maxActiveOrders  = 4;
        [SerializeField] private float orderInterval    = 30f;  // segundos entre nuevos pedidos
        [SerializeField] private float premiumChance    = 0.2f;
        [SerializeField] private float rareChance       = 0.05f;

        // ── Estado ─────────────────────────────────────────────
        private List<OrderData> _activeOrders = new List<OrderData>();
        private float           _orderTimer   = 0f;
        private int             _orderCounter = 0;

        // ── Nombres de clientes ficticios ──────────────────────────
        private static readonly string[] ClientNames =
        {
            "Mira la Brumera", "Old Toadsworth", "Chef Nubillo",
            "Lady Vapour",     "Finn el Carguero", "Seta-San",
            "Capitana Rosal",  "El Mielero"
        };

        // ── Eventos ────────────────────────────────────────────
        public static event System.Action<OrderData>       OnOrderAdded;
        public static event System.Action<OrderData>       OnOrderCompleted;
        public static event System.Action<OrderData>       OnOrderExpired;
        public static event System.Action<List<OrderData>> OnOrdersUpdated;

        // ── Singleton ──────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            GenerateOrder(); // Primer pedido inmediato
        }

        private void Update()
        {
            _orderTimer += Time.deltaTime;
            if (_orderTimer >= orderInterval && _activeOrders.Count < maxActiveOrders)
            {
                GenerateOrder();
                _orderTimer = 0f;
            }

            TickOrderTimers();
        }

        // ── Generar pedido ─────────────────────────────────────────
        private void GenerateOrder()
        {
            var db = RecipeDatabase.Instance;
            if (db == null) return;

            var available = db.GetUnlockedRecipes();
            if (available.Count == 0) return;

            var recipe = available[Random.Range(0, available.Count)];

            // Tipo de pedido
            OrderType type = OrderType.Normal;
            float roll = Random.value;
            if (roll < rareChance)         type = OrderType.Rare;
            else if (roll < premiumChance) type = OrderType.Premium;

            float rewardMult = type == OrderType.Rare ? 3f : (type == OrderType.Premium ? 1.8f : 1f);

            var order = new OrderData
            {
                orderId    = $"order_{_orderCounter++}",
                clientName = ClientNames[Random.Range(0, ClientNames.Length)],
                recipeId   = recipe.recipeId,
                dishName   = recipe.resultItemName,
                reward     = Mathf.RoundToInt(recipe.sellPrice * rewardMult),
                fameReward = recipe.fameReward,
                type       = type,
                timeLimit  = type == OrderType.Rare ? 60f : 120f,
                status     = OrderStatus.Pending
            };

            _activeOrders.Add(order);
            OnOrderAdded?.Invoke(order);
            OnOrdersUpdated?.Invoke(_activeOrders);
            Debug.Log($"[Orders] Nuevo pedido [{type}]: {order.dishName} para {order.clientName}");
        }

        // ── Completar pedido ─────────────────────────────────────────
        public bool TryFulfillOrder(string orderId)
        {
            var order = _activeOrders.Find(o => o.orderId == orderId);
            if (order == null || order.status != OrderStatus.Pending) return false;

            var data = GameData.Instance;
            if (data == null) return false;

            // Verificar que tiene el platillo
            if (!data.HasItem(order.recipeId.Replace("receta_", "platillo_")))
            {
                // Buscar directamente por resultItemId de la receta
                var recipe = RecipeDatabase.Instance?.GetRecipe(order.recipeId);
                if (recipe == null || !data.HasItem(recipe.resultItemId))
                {
                    Debug.LogWarning("[Orders] No tienes el platillo listo.");
                    return false;
                }
                data.RemoveItem(recipe.resultItemId);
            }

            order.status = OrderStatus.Completed;
            data.AddCoins(order.reward);
            data.fameCookery += order.fameReward;
            data.totalOrdersCompleted++;

            _activeOrders.Remove(order);
            OnOrderCompleted?.Invoke(order);
            OnOrdersUpdated?.Invoke(_activeOrders);
            Debug.Log($"[Orders] ✅ Pedido completado: +{order.reward} monedas, +{order.fameReward} fama");
            return true;
        }

        // ── Timer de expiración ──────────────────────────────────────
        private void TickOrderTimers()
        {
            var toRemove = new List<OrderData>();
            foreach (var o in _activeOrders)
            {
                if (o.timeLimit <= 0) continue;
                o.timeElapsed += Time.deltaTime;
                if (o.timeElapsed >= o.timeLimit)
                {
                    o.status = OrderStatus.Expired;
                    toRemove.Add(o);
                    OnOrderExpired?.Invoke(o);
                    Debug.Log($"[Orders] Pedido expirado: {o.dishName}");
                }
            }
            if (toRemove.Count > 0)
            {
                foreach (var o in toRemove) _activeOrders.Remove(o);
                OnOrdersUpdated?.Invoke(_activeOrders);
            }
        }

        public List<OrderData> GetActiveOrders() => _activeOrders;
    }
}
