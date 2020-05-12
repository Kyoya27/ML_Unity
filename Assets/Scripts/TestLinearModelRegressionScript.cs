
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestLinearModelRegressionScript : MonoBehaviour
{
    


    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;

    public void TransfoSoftThenTrain()
    {
        foreach (var trainSpheres in trainSpheresTransforms)
        {
            if(trainSpheres.position.y > 0 && trainSpheres.position.x <1){
                trainSpheres.position= new Vector3(
                    1f,
                    trainSpheres.position.y,
                    trainSpheres.position.z
                );
            }
            
            else if(trainSpheres.position.y < 0 && trainSpheres.position.x >0.8){
                trainSpheres.position= new Vector3(
                    0f,
                    trainSpheres.position.y,
                    trainSpheres.position.z
                );
            }
        }
    }
    public void TransfoCrossThenTrain()
    {
        
        foreach (var trainSpheres in trainSpheresTransforms)
        {
            if(trainSpheres.position.y>5)
            {
                trainSpheres.position= new Vector3(
                    trainSpheres.position.x,
                    trainSpheres.position.y*(-1f),
                    trainSpheres.position.z
                );
            }
        }

    }
    public void TransfoXORThenTrain()
    {
        foreach (var trainSpheres in trainSpheresTransforms)
        {
            if(trainSpheres.position.y > 0 && trainSpheres.position.x <1){
                trainSpheres.position= new Vector3(
                    (float)Math.Pow(trainSpheres.position.x + trainSpheres.position.z, 2),
                    trainSpheres.position.y,
                    (float)Math.Pow(trainSpheres.position.x + trainSpheres.position.z, 2)
                );
            }
        }

    }


    public unsafe  void TrainAndTest()
    {
        Debug.Log("Training and Testing");

        var Y = new double[trainSpheresTransforms.Length];
        IntPtr model;

        var linear_inputs = new double[trainSpheresTransforms.Length *2];

        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            Y[i] = trainSpheresTransforms[i].position.y;
            linear_inputs[i*2] = trainSpheresTransforms[i].position.x;
            linear_inputs[(i*2)+1] = trainSpheresTransforms[i].position.z;
        }

        // Create Model
        model = VisualStudioLibWrapper.linear_model_create(2);
        // Train Model
        VisualStudioLibWrapper.linear_model_train_regression(model, linear_inputs, trainSpheresTransforms.Length, 2, Y, Y.Length,1000000, 0.01f);

        // For each testSphere : Predict 
        foreach (var testSpheres in testSpheresTransforms)
        {
            double[]input = { testSpheres.position.x, testSpheres.position.z};
            float y = (float)VisualStudioLibWrapper.linear_model_predict_regression(model, input, 2);
            testSpheres.position = new Vector3(
                testSpheres.position.x,
                y,
                testSpheres.position.z
            );
        }
    }
}
