using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLinearModelClassificationScript : MonoBehaviour
{

    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;

    private static double[][] Concat(double[,] array1, double[,] array2)
    {
        double[,] tmp = new double[array1.GetLength(0) + array2.GetLength(0)][];

        array1.CopyTo(tmp, 0);
        array2.CopyTo(tmp, array1.Length);

        return tmp;
    }

    private static double[][] AppendTo2D(double[][] array, double x, double y)
    {
        double[][] result = new double[array.GetLength(0) +1][];
        int i;
        for (i = 0; i < array.Length ; i++)
        {
            result[i][0] = array[i][0];
            result[i][1] = array[i][1];
        }
        result[i][0] = x;
        result[i][1] = y;

        return result;
    }

    public void TrainAndTest()
    {
        Debug.Log("Training and Testing");

        int count = 0;
        // Créer dataset_inputs
        // Créer dataest_expected_outputs

        double[] Y = new double[trainSpheresTransforms.GetLength(0)];
        double[,] blue_points = new double[1,2];
        double[,] red_points = new double[2,2];

        foreach (var trainSphere in trainSpheresTransforms)
        {
            if(trainSphere.position.y < 0)
            {
                //blue_points[count][] = AppendTo2D(blue_points, trainSphere.position.x, trainSphere.position.z);
                Y[count] = -1;
            }
            else
            {
                //red_points = AppendTo2D(red_points, trainSphere.position.x, trainSphere.position.z);
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
       
       

        double[][] inputs = new double[blue_points.GetLength(0) + red_points.GetLength(0)][];

        inputs = blue_points.CopyTo(inputs, 0);
        red_points.CopyTo(inputs, blue_points.GetLength(0));

        //double[][] inputs = Concat(blue_points, red_points);

        Debug.Log(blue_points.GetLength(0));
        Debug.Log(red_points.GetLength(0));
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
