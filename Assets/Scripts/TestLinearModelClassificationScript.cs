
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestLinearModelClassificationScript : MonoBehaviour
{
    


    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;


    public unsafe  void TrainAndTest()
    {
        Debug.Log("Training and Testing");

        int count = 0;
        int blue_count = 0;
        int red_count = 0;
        IntPtr model;

        // Créer dataset_inputs
        // Créer dataest_expected_outputs

        var Y = new double[trainSpheresTransforms.Length];
        //double* Y;

        foreach (var trainSphere in trainSpheresTransforms)
        {
            if(trainSphere.position.y < 0)
                blue_count++;
            else
                red_count++;
        }
        
        double[,] blue_points = new double[blue_count,2];
        double[,] red_points = new double[red_count,2];
        count = 0;
        blue_count = 0;
        red_count = 0;
        foreach (var trainSphere in trainSpheresTransforms)
        {
            if(trainSphere.position.y < 0)
            {
                blue_points[blue_count,0] = trainSphere.position.x;
                blue_points[blue_count,0] = trainSphere.position.z;
                blue_count++;
                Y[count] = -1;
            }
            else
            {
                red_points[red_count,0] = trainSphere.position.x;
                red_points[red_count,0] = trainSphere.position.z;
                red_count++;
                Y[count] = 1;
            }
            count++;
        }

        double[,] X = new double[blue_points.GetLength(0) + red_points.GetLength(0),3];
        for (int i = 0; i< X.GetLength(0); i++)
        {
            if (i < blue_points.GetLength(0))
            {
                X[i,0] = 1;
                X[i,1] = blue_points[i,0];
                X[i,2] = blue_points[i,1];

            }
            else
            {
                int j = i - blue_points.GetLength(0);
                X[i,0] = 1;
                X[i,1] = red_points[j,0];
                X[i,2] = red_points[j,1];
            }
        }

        var linear_inputs = new double[trainSpheresTransforms.Length *2];

        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            linear_inputs[i] = X[i,1];
            linear_inputs[i+1] = X[i,2];

        }

        // Create Model
        model = VisualStudioLibWrapper.linear_model_create(2);
        // Train Model
        VisualStudioLibWrapper.linear_model_train_classification(model, linear_inputs, trainSpheresTransforms.Length, 2, Y, Y.Length,1000, 0.01f);
        //double* model = {-0.4, 0.4, 0.2};
        //double[] model = new double [] {-0.4, 0.4, 0.2};

        // For each testSphere : Predict 
        foreach (var testSpheres in testSpheresTransforms)
        {
            double[]input = {testSpheres.position.x, testSpheres.position.z};
            //double y = (float)(-model[1] / model[2] * testSpheresTransform.position.x - model[0] / model[2]);
            float y = (float)VisualStudioLibWrapper.linear_model_predict_classification(model, input, 2);
            testSpheres.position = new Vector3(
                testSpheres.position.x,
                y,
                testSpheres.position.z
            );
        }

        // Delete Model
        //linear_model_delete(model);
        //model = null;
    }
}
