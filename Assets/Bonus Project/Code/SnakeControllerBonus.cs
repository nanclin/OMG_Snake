using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeControllerBonus : MonoBehaviour {

    public float MoveSpeed = 5;
    public float SteerSpeed = 180;
    public float BodySpeed = 5;
    public int Gap = 10;

    public GameObject Head;
    public GameObject BodyPrefab;
    public GameObject FoodPrefab;

    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();

    // Start is called before the first frame update
    void Start() {

        // start with 10 body parts
        //for (int i = 0; i < 10; i++) {
        //    GrowSnake();
        //}

        SpawnFood();
    }

    // Update is called once per frame
    void Update() {

#if DEBUG
        if (Input.GetKeyDown(KeyCode.Plus))
            Gap++;

        if (Input.GetKeyDown(KeyCode.Minus))
            Gap--;

        if (Input.GetKeyDown(KeyCode.G))
            GrowSnake();
#endif

        // move forward
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        // steer
        float steerDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerDirection * SteerSpeed * Time.deltaTime);

        // store position history
        PositionsHistory.Insert(0, transform.position);

        // move body parts
        int index = 0;
        foreach (var body in BodyParts) {
            Vector3 point = PositionsHistory[Mathf.Min(index * Gap + Gap / 2, PositionsHistory.Count - 1)];

            point += Vector3.up * Mathf.Sin(index * 3.14f + Time.time * 15) * 0.5f;
            Vector3 moveDirection = point - body.transform.position;

            body.transform.position += moveDirection * BodySpeed * Time.deltaTime;

            point.y = body.transform.position.y;
            body.transform.LookAt(point);
            index++;
        }

        Head.transform.localPosition = Vector3.up * Mathf.Sin(-1 * 3.14f + Time.time * 15) * 0.1f;
    }

    private void GrowSnake() {
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);
    }

    private void SpawnFood() {

        // get random values on x, z axis
        float x = Random.Range(0, 10);
        float y = 0;
        float z = Random.Range(0, 10);

        // generate food
        GameObject food = Instantiate(FoodPrefab);

        // set position
        food.transform.position = new Vector3(x, y, z);
    }

    private void OnTriggerEnter(Collider other) {
        // delete collected food
        Destroy(other.gameObject);

        // increase snake in size
        GrowSnake();

        // spawn next food
        SpawnFood();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        foreach (var p in PositionsHistory) {
            Gizmos.DrawSphere(p, 0.1f);
        }
    }
}
