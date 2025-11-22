using System.Collections;

public interface IDrag
{
    public void OnDragStart();
    public IEnumerator OnDragUpdate();
    public void OnDragRelease();
}
