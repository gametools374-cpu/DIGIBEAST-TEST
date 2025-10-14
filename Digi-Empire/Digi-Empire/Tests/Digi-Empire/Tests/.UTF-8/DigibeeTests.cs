public class SimplePool<T> where T : Component
{
    readonly T prefab;
    readonly Stack<T> stack = new Stack<T>();
    public SimplePool(T prefab) { this.prefab = prefab; }
    public T Get()
    {
        if (stack.Count > 0) { var obj = stack.Pop(); obj.gameObject.SetActive(true); return obj; }
        return Object.Instantiate(prefab);
    }
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        stack.Push(obj);
    }
}
