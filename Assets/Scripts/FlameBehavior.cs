using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBehavior : MonoBehaviour
{
    public Vector3 direction;

    private float speed = 30;
    private float lifespan = 1f;
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;

        direction.x *= Random.Range(.75f, 1.25f);
        direction.y *= Random.Range(.4f, .6f);
        direction.z *= Random.Range(.75f, 1.25f);
    }

    // Update is called once per frame
    void Update()
    {
        lifespan -= Time.deltaTime;
        float a = lifespan / 1f;

        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, a);

        transform.position += direction * speed * Time.deltaTime;

        float scaler = (1f - lifespan)/1f;
        transform.localScale = AnimMath.Lerp(Vector3.one, Vector3.one * 3, scaler);

        if (lifespan <= 0) Destroy(gameObject);
    }
}
