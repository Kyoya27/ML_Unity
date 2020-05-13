
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestLinearModelClassificationScript : MonoBehaviour
{
    


    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;

    private int transfo = -1;


    public void TransfoSoftThenTrain()
    {
        foreach (var trainSpheres in trainSpheresTransforms)
        {
            var posX = trainSpheres.position.x;
            var posZ = trainSpheres.position.z;
            if(posX >=5 && posZ >1 && posZ <2){
                trainSpheres.position= new Vector3(
                    -5f,
                    trainSpheres.position.y,
                    trainSpheres.position.z
                );
            }
            
            if(posX <= -2 && posZ <8 && posZ >7){
                trainSpheres.position= new Vector3(
                    3f,
                    trainSpheres.position.y,
                    trainSpheres.position.z
                );
            }
        }
        foreach (var testSpheres in testSpheresTransforms)
        {
            var posX = testSpheres.position.x;
            var posZ = testSpheres.position.z;
            if(posX >=5 && posZ >1 && posZ <2){
                testSpheres.position= new Vector3(
                    -5f,
                    testSpheres.position.y,
                    testSpheres.position.z
                );
            }
            
            if(posX <= -2 && posZ <8 && posZ >7){
                testSpheres.position= new Vector3(
                    3f,
                    testSpheres.position.y,
                    testSpheres.position.z
                );
            }
        }
    }

    public void TransfoCrossThenTrain()
    {
        
        foreach (var trainSpheres in trainSpheresTransforms)
        {
            
            var posX = Math.Abs(trainSpheres.position.x);
            var posZ = Math.Abs(trainSpheres.position.z);
            if(trainSpheres.position.y > 0){
                trainSpheres.position= new Vector3(
                    trainSpheres.position.x-5f,
                    trainSpheres.position.y,
                    trainSpheres.position.z
                );
            }
        }
        foreach (var testSpheres in testSpheresTransforms)
        {
            var posX = Math.Abs(testSpheres.position.x);
            var posZ = Math.Abs(testSpheres.position.z);
            if(testSpheres.position.y > 0){
                testSpheres.position= new Vector3(
                    testSpheres.position.x-5f,
                    testSpheres.position.y,
                    testSpheres.position.z
                );
            }
        }

        /*foreach (var trainSpheres in trainSpheresTransforms)
        {
            
            var posX = Math.Abs(trainSpheres.position.x);
            var posZ = Math.Abs(trainSpheres.position.z);
            if(trainSpheres.position.y > 0){
                trainSpheres.position= new Vector3(
                    trainSpheres.position.x-5f,
                    trainSpheres.position.y,
                    trainSpheres.position.z
                );
            }
        }
        foreach (var testSpheres in testSpheresTransforms)
        {
            var posX = Math.Abs(testSpheres.position.x);
            var posZ = Math.Abs(testSpheres.position.z);
            if(testSpheres.position.y > 0){
                testSpheres.position= new Vector3(
                    testSpheres.position.x-5f,
                    testSpheres.position.y,
                    testSpheres.position.z
                );
            }
        }*/

    }

    public void TransfoXORThenTrain()
    {
        foreach (var trainSpheres in trainSpheresTransforms)
        {
            if(trainSpheres.position.x < 2 && trainSpheres.position.z > 1 && trainSpheres.position.z < 2){
                trainSpheres.position= new Vector3(
                    5f,
                    trainSpheres.position.y,
                    trainSpheres.position.z
                );
            }
            else if(trainSpheres.position.x > 5.9 && trainSpheres.position.z > 0 && trainSpheres.position.z < 1){
                trainSpheres.position= new Vector3(
                    -3.2f,
                    trainSpheres.position.y,
                    trainSpheres.position.z
                );
            }
        }

    }


    public unsafe  void TrainAndTest()
    {
        Debug.Log("Training and Testing");

        int count = 0;
        int blue_count = 0;
        int red_count = 0;

        // Créer dataset_inputs
        // Créer dataest_expected_outputs

        var Y = new double[trainSpheresTransforms.Length];
        IntPtr model;

        var linear_inputs = new double[trainSpheresTransforms.Length *2];

        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            Y[i] = trainSpheresTransforms[i].position.y > 0 ? 1 : -1;
            linear_inputs[i*2] = trainSpheresTransforms[i].position.x;
            linear_inputs[(i*2)+1] = trainSpheresTransforms[i].position.z;
        }

        // Create Model
        model = VisualStudioLibWrapper.linear_model_create(2);
        // Train Model
        VisualStudioLibWrapper.linear_model_train_classification(model, linear_inputs, trainSpheresTransforms.Length, 2, Y, Y.Length,1000000, 0.01f);
        //double* model = {-0.4, 0.4, 0.2};
        //double[] model = new double [] {-0.4, 0.4, 0.2};

        // For each testSphere : Predict 
        foreach (var testSpheres in testSpheresTransforms)
        {
            double[]input = { testSpheres.position.x, testSpheres.position.z};
            //double y = (float)(-model[1] / model[2] * testSpheresTransform.position.x - model[0] / model[2]);
            float y = (float)VisualStudioLibWrapper.linear_model_predict_classification(model, input, 2);
            testSpheres.position = new Vector3(
                testSpheres.position.x,
                y,
                testSpheres.position.z
            );
        }

        // Delete Model
        VisualStudioLibWrapper.clearArray(model);
    }
}
