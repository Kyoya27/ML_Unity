using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLinearModelClassificationScript : MonoBehaviour
{

    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;


    public void TrainAndTest()
    {
        Debug.Log("Training and Testing");

        int count = 0;
        int blue_count = 0;
        int red_count = 0;
        // Créer dataset_inputs
        // Créer dataest_expected_outputs

        double[] Y = new double[trainSpheresTransforms.GetLength(0)];

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

        /*double[,] blue_points = new double[,] {
            { 0.35, 0.5 }
        };
        double[,] red_points = new double[,] {
            { 0.6, 0.6 },
            { 0.55, 0.7 }
        };*/

        double[,] inputs = new double[blue_points.GetLength(0) + red_points.GetLength(0),2];

        for (int i = 0; i < inputs.GetLength(0); i++)
        {
            if(i < blue_points.GetLength(0))
            {
                inputs[i,0] = blue_points[i,0];
                inputs[i,1] = blue_points[i,1];
            }
            else
            {
                int j = i - blue_points.GetLength(0);
                inputs[i,0] = red_points[j,0];
                inputs[i,1] = red_points[j,1];
            }
        }
        //double[][] inputs = Concat(blue_points, red_points);

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

        //test
        //double[] Y = new double []{1, -1, -1};

        // Create Model
        //double* model = linear_model_create(len(inputs[0]));

        // Train Model
        //linear_model_train_classification(model, inputs, Y, iterations_count = 1000);
        //double* model = {-0.4, 0.4, 0.2};
        double[] model = new double [] {-0.4, 0.4, 0.2};

        count = 0;
        // For each testSphere : Predict 
        foreach (var testSpheresTransform in testSpheresTransforms)
        {
            float y = (float)(-model[1] / model[2] * testSpheresTransform.position.x - model[0] / model[2]);
            testSpheresTransform.position = new Vector3(
                testSpheresTransform.position.x,
                y,
                testSpheresTransform.position.z
            );

            count++;
        }

        // Delete Model
        //linear_model_delete(model);
    }
}
