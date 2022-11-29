using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Pool
{

    // public GameObject Prefab {get => prefab; }
    public GameObject Prefab => prefab;

    public int Size => size;

    public int RuntimeSize => queue.Count;

    [SerializeField] GameObject prefab;

    [SerializeField] int size = 1;  //size of pool

    Queue<GameObject> queue;

    Transform parent;

    public void Initialize(Transform parent){
        queue = new Queue<GameObject>();

        this.parent = parent;

        for(var i = 0; i < size; i ++){
            queue.Enqueue(Copy());
        }
    }

    //create prepared objects
    GameObject Copy(){
        //make it can be started and working at any time
        var copy = GameObject.Instantiate(prefab, parent);

        copy.SetActive(false);

        return copy;
    }

    //get an available object from pool
    GameObject AvailableObject(){
        GameObject availableObject = null;

        if(queue.Count > 0 && !queue.Peek().activeSelf){
            availableObject = queue.Dequeue();
        }else{
            availableObject = Copy();
        }

        queue.Enqueue(availableObject);

        return availableObject;
    }

    //active the available object
    public GameObject preparedObject(){
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);

        return preparedObject;
    } 

    public GameObject preparedObject(Vector3 position){
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);

        preparedObject.transform.position = position;

        return preparedObject;
    } 

    public GameObject preparedObject(Vector3 position, Quaternion rotation){
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);

        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;

        return preparedObject;
    } 

    public GameObject preparedObject(Vector3 position, Quaternion rotation, Vector3 localScale){
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);

        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;
        preparedObject.transform.localScale = localScale;

        return preparedObject;
    } 

    // //return object to the pool
    // public void Return(GameObject gameObject){
    //     queue.Enqueue(gameObject);
    // }
}
