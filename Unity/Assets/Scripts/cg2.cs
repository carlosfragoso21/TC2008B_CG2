//  Tarea CG2
//  15/11/2023
//  Carlos Stefano Fragoso Martinez - A01028113
//---------------------------------------------------------------------------
// This script is used in unity project in order to initializate wheels,    -
// and apply transforms in car and wheels meshes.                           -
// Purpouse is to connect wheels to car with the script.                    -
//---------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cg2 : MonoBehaviour
{
    [SerializeField] Vector3 speed; // x,y,z parameters
    [SerializeField] GameObject prefabsDeRuedas; //wheels prefab
    [SerializeField] List<Vector3> wheels; // wheels list

    List<Vector3[]> listBase; //wheels base list
    List<Vector3[]> listNew; // wheels new list
    Vector3[] listBaseC; //car base list
    Vector3[] listNewC; //car new list
    List<Mesh> listMesh; // wheels mesh list
    Mesh carMesh; // car mesh

    // Start is called before the first frame update
    void Start(){
        carMesh = GetComponentInChildren<MeshFilter>().mesh; // Gets the car's mesh (this script was generated from car, so we get mesh directly)
        listBaseC = carMesh.vertices; 
        listNewC = new Vector3[listBaseC.Length];
        int i = 0;
        listMesh = new List<Mesh>();
        listBase = new List<Vector3[]>();
        listNew = new List<Vector3[]>();
        foreach (Vector3 wheelPosition in wheels){ // loop to each wheel
            GameObject wheel = Instantiate(prefabsDeRuedas, new Vector3(0, 0, 0), Quaternion.identity); //initilization of it
            listMesh.Add(wheel.GetComponentInChildren<MeshFilter>().mesh); // Gets actual wheel mesh
            listBase.Add(listMesh[i].vertices);
            listNew.Add(new Vector3[listBase[i].Length]);
            i++;
        }
    }

    // Update is called once per frame
    void Update(){
        Matrix4x4 Car = carComposite(); //Gets matrix with translation / and rotation / transformations
        for (int i = 0; i < listNewC.Length; i++){ // loop for each vertice
            Vector4 temp = new Vector4(listBaseC[i].x,listBaseC[i].y,listBaseC[i].z,1); 
            listNewC[i] = Car * temp; // Apply transformation to car vertices
        }
        carMesh.vertices = listNewC; //Refresh car mesh with new vertices
        carMesh.RecalculateNormals(); //normals
        
        int index = 0;
        foreach (Vector3 wheelPosition in wheels){
            Matrix4x4 move = HW_Transforms.TranslationMat(wheelPosition.x, wheelPosition.y, wheelPosition.z);
            Matrix4x4 rotate_f = HW_Transforms.RotateMat(90, AXIS.Y); // Fixed rotation for correct Y orientation relative to the car.
            Matrix4x4 rotate = HW_Transforms.RotateMat(0 * Time.time, AXIS.Z); // initial rotation (simulation of movement)
            if(speed.x < 0){ // Condition to provide the correct Z orientation to the wheel relative to the direction and movement of the car.
                rotate = HW_Transforms.RotateMat(-90 * Time.time, AXIS.Z); 
            }else if(speed.x > 0){
                rotate = HW_Transforms.RotateMat(90 * Time.time, AXIS.Z);
            }
            for (int i = 0; i < listNew[index].Length; i++){ //[][] vertices list
                Vector4 temp = new Vector4(listBase[index][i].x, listBase[index][i].y, listBase[index][i].z, 1);
                listNew[index][i] = Car * move * rotate * rotate_f * temp; //Apply transformations
            }
            listMesh[index].vertices = listNew[index];
            listMesh[index].RecalculateNormals();
            index++;
        }       
    }
    // Function to calculate the composite matrix for the car
    Matrix4x4 carComposite(){
        Matrix4x4 Tmovement;
        Matrix4x4 composite;
        if(speed.x == 0){
            Tmovement = HW_Transforms.TranslationMat(0, 0, 0); // No movement if speed.x is 0
        }else{
            Tmovement = HW_Transforms.TranslationMat(speed.x * Time.time, 0, speed.z * Time.time); // movement based on speed.x and speed.z
            if(speed.z != 0){
            float Theta = ( Mathf.Atan2(speed.z, speed.x) * Mathf.Rad2Deg ) + 180; //get degree for car rotation.
            Matrix4x4 rotate = HW_Transforms.RotateMat(Theta, AXIS.Y);
            composite = Tmovement * rotate;
            return composite;
            }
        }
        composite = Tmovement;
        return composite;
    }
}
