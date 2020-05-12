
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestLinearModelRegressionScript : MonoBehaviour
{
    


    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;


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
