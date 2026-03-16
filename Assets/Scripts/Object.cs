using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Object : Identity
{

    [Header("Object")]
    public Rigidbody2D _rb {get; protected set;}
    public Animator _animator{ get; protected set; }
    public Collider2D _collider { get; protected set; }

    [Header("Variable")]
    public int MaxHP;
    public float MovementSpeed;

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

    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
