using UnityEngine;

public class Identity : MonoBehaviour
{
        [Header("Identity")]
        public string Name;
        public int positionX;
        public int positionY;

        public void Start()
        {
            SetUp();
        }
        public virtual void SetUp()
        {
            positionX = (int)transform.position.x;
            positionY = (int)transform.position.y;
        }
    public void SetName(string name)
    {
        Name = name;
    }
        public void PrintInfo()
        {
            Debug.Log("created " + Name + " at " + positionX + ":" + positionY);
        }
}
