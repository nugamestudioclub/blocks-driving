using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    private const int MAX_DEGREES = 360;

    [SerializeField]
    [Range(0, float.MaxValue)]
    private float resetSpeed = 50.0f;

    private float angle = 0.0f;

    [SerializeField]
    [Range(0, MAX_DEGREES / 2)]
    private float maxAngle = 90.0f;
    public float MaxAngle => maxAngle;

    private bool isOver = false;

    private bool isHeld = false;

    private static float AngleBetween(Vector2 p1, Vector2 p2)
    {
        return Mathf.Rad2Deg * Mathf.Atan2(p2.y - p1.y, p2.x - p1.x);
    }

    public static float Sign(float angle)
    {
        return angle < (MAX_DEGREES / 2) ? 1 : -1;
    }

    private float ClampToMax(float angle)
    {
        return Sign(angle) > 0
            ? Mathf.Min(angle, maxAngle)
            : Mathf.Max(angle, MAX_DEGREES - maxAngle);
    }

    private static float ClampToZero(float angle)
    {
        const float EPSILON = 0.01f;

        return Abs(angle) > EPSILON ? angle : 0;
    }

    public static float Abs(float angle)
    {
        return Sign(angle) > 0 ? angle : MAX_DEGREES - angle;
    }

    private static bool IsZero(float angle)
    {
        return Mathf.Approximately(Abs(angle), 0);
    }

    private void Rotate(float deltaZ)
    {
        transform.Rotate(0.0f, 0.0f, deltaZ);
    }
    private float MouseAngle()
    {
        var pos = World.Instance.InteriorCamera.ScreenToWorldPoint(Input.mousePosition);

        return AngleBetween(
            new Vector2(pos.x, pos.y),
            new Vector2(transform.position.x, transform.position.y)
        );
    }

    void Update()
    {
        if (!(isOver && isHeld))
        {
            float z = transform.rotation.eulerAngles.z;
            float deltaZ = Sign(z) * resetSpeed * Time.deltaTime;

            z = ClampToZero(transform.rotation.eulerAngles.z - deltaZ);
            deltaZ = z - transform.rotation.eulerAngles.z;

            Rotate(deltaZ);
            isHeld = IsZero(transform.rotation.eulerAngles.z);
        }
    }

    void OnMouseOver()
    {
        isOver = true;
    }

    void OnMouseExit()
    {
        isOver = false;
        World.Instance.Hand.Open();
    }

    void OnMouseDown()
    {
        isHeld = true;
        angle = MouseAngle();
        World.Instance.Hand.Close();
    }

    void OnMouseUp()
    {
        isHeld = false;
        World.Instance.Hand.Open();
    }

    void OnMouseDrag()
    {
        if (isOver && isHeld)
        {
            float mouseAngle = MouseAngle();
            float deltaZ = mouseAngle - angle;
            float z = ClampToMax(transform.rotation.eulerAngles.z + deltaZ);

            deltaZ = z - transform.rotation.eulerAngles.z;

            Rotate(deltaZ);

            angle = mouseAngle;
        }
    }
}