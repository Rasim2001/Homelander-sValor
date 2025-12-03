namespace BuildProcessManagement.ResourceElements
{
    public class StoneDestroyResource : DestroyResource
    {
        public override void DestroyElement()
        {
            base.DestroyElement();

            Destroy(gameObject);
        }
    }
}