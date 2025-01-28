using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    private const string ROAD_LAYER = "Road";
    [SerializeField]
    private Camera cam;
    [SerializeField] private Road[] prefabs;
    private Road _road = null;
    private Road _hitRoad = null;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!_road)
            {
                _road = Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform.position, Quaternion.identity);
                _road.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                for (int i = 0; i < _road.transform.childCount; i++)
                {
                    _road.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                }
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);
            Debug.DrawRay(hit.point, transform.up *hit.distance, Color.blue);
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(ROAD_LAYER))
            {
                if (!_hitRoad || _hitRoad.gameObject != hit.collider.gameObject)
                {
                    _hitRoad = hit.collider.gameObject.GetComponentInParent<Road>();
                }

                RoadSocket hitSocketPosition = _hitRoad.GetCloserSocket(hit.point);
                _road.LockInSocket(hitSocketPosition);
            }
            else
            {
                _road.transform.position = hit.point;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _road.gameObject.layer = LayerMask.NameToLayer(ROAD_LAYER);
            _road.gameObject.layer = LayerMask.NameToLayer(ROAD_LAYER);
            for (int i = 0; i < _road.transform.childCount; i++)
            {
                _road.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(ROAD_LAYER);
            }
            _road = null;
            _hitRoad = null;
        }

        if (Input.GetKeyDown(KeyCode.R) && _road)
        {
            _road.transform.Rotate(Vector3.up, 90);
        }
    }
}