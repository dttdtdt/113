using System.Collections.Generic;
using GameSystem.UISystem.UIBound;
using UnityEngine;

namespace GameSystem.UISystem.UIManager
{
    public class UIPool : MonoBehaviour
    {
        [SerializeField] private CardUIBound itemPrefab;
        [SerializeField] private RectTransform contentRoot;
        [SerializeField, Min(0)] private int poolSize = 10;
        private readonly Queue<CardUIBound> available = new();
        private readonly HashSet<CardUIBound> owned = new();
        private readonly HashSet<CardUIBound> rented = new();
        private Transform storage;
        private bool initialized;

        public int ActiveCount => rented.Count;
        public int TotalCount => owned.Count;

        private void Awake() => Initialize();

        private void Initialize()
        {
            if (initialized || itemPrefab == null || contentRoot == null) return;
            storage = new GameObject("PooledCards", typeof(RectTransform)).transform;
            storage.SetParent(transform, false);
            storage.gameObject.SetActive(false);
            initialized = true;
            for (int i = 0; i < poolSize; i++) available.Enqueue(CreateItem());
        }

        private CardUIBound CreateItem()
        {
            var view = Instantiate(itemPrefab, storage, false);
            view.gameObject.SetActive(false);
            owned.Add(view);
            return view;
        }

        public CardUIBound Get()
        {
            Initialize();
            if (!initialized) return null;
            CardUIBound view = null;
            while (available.Count > 0 && view == null) view = available.Dequeue();
            if (view == null) view = CreateItem();
            rented.Add(view);
            var rect = (RectTransform)view.transform;
            rect.SetParent(contentRoot, false);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.anchoredPosition = Vector2.zero;
            view.gameObject.SetActive(true);
            return view;
        }

        public void ReturnToPool(CardUIBound view)
        {
            // 只有本池借出的实例才能归还，同一实例不能重复入队。
            if (view == null || !owned.Contains(view) || !rented.Remove(view)) return;
            view.Data = null;
            view.gameObject.SetActive(false);
            view.transform.SetParent(storage, false);
            view.transform.localScale = Vector3.one;
            view.transform.localRotation = Quaternion.identity;
            ((RectTransform)view.transform).anchoredPosition = Vector2.zero;
            available.Enqueue(view);
        }
    }
}
