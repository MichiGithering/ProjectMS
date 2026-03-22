using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Objects : Identity
{

    [Header("Object")]
    public Rigidbody2D _rb {get; protected set;}
    public Animator _animator{ get; protected set; }
    public Collider2D _collider { get; protected set; }

    [Header("Variable")]
    public int MaxHP;
    public float MaxMovementSpeed;

    [Header("Mirage")]
    public bool Mirage = false;
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        if( _rb == null )
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<CircleCollider2D>();
        }

        _rb.gravityScale = 0;
        _rb.freezeRotation = true;
    }
    public override void Start()
    {
        base.Start();
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Mirage)
        {
            Player playerScript = collision.GetComponent<Player>();

            if (playerScript != null)
            {
                Debug.Log("Player flew into a Mirage! It vanished!");
                Destroy(gameObject);
                return;
            }
        }
    }
}
