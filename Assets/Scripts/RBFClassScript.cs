
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class RBFClassScript : MonoBehaviour
{
    
    /*
    W
    x
    z*/
    public struct Projection
    {
        public double[] W;
        public double[] X;
    }

    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;

    public double getRBFValue(double gamma, Vector3 x1, Vector3 x2)
    {
        double x = Math.Pow(x1.x - x2.x, 2);
       // double y = Math.Pow(x1.y - x2.y, 2);
        double z = Math.Pow(x1.z - x2.z, 2);
        double normalized = Math.Sqrt(x /*+ y*/ + z);
        double pow = Math.Pow(normalized, 2);
        double gam = -1 * gamma * pow;
        return Math.Exp(gam);
    }


    public unsafe void RBFEntries()
    {
        double epsilon = 0.8;

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

        var multiY = new double[Y.Length, 1];
        var Ox = new double[trainSpheresTransforms.Length, trainSpheresTransforms.Length];
        //var Oz = new double[trainSpheresTransforms.Length,trainSpheresTransforms.Length];

        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            for (int j = 0; j < trainSpheresTransforms.Length; j++)
            {
                Ox[i,j] = getRBFValue(epsilon, trainSpheresTransforms[i].position, trainSpheresTransforms[j].position);
                //Oz[i,j] = getRBFValue(epsilon, trainSpheresTransforms[i].position.z, trainSpheresTransforms[j].position.z);
            }
            multiY[i, 0] = Y[i];
        }

        var l = trainSpheresTransforms.Length;
        Matrix<double> OxMatrix = DenseMatrix.OfArray(Ox);
        for(int i = 0; i < l; i++) {
            string str = "";
            for(int j = 0; j < l; j++) {
                // Debug.Log(i + " " + j + ": " + Ox[i, j]);
                str += OxMatrix[i, j] + "/ ";
            }

            Debug.Log(str);
        }
        //Matrix<double> OzMatrix = DenseMatrix.OfArray(Oz);
        //Matrix<double> YMatrix = DenseMatrix.OfArray(Y);
        // Debug.Log(OxMatrix.RowCount);
        // Debug.Log(OxMatrix.ColumnCount);

        Matrix<double> OxInv = OxMatrix.Inverse();
        Matrix<double> mY = DenseMatrix.OfArray(multiY);

        Matrix<double> resMatrix = OxInv.Multiply(mY);
        Debug.Log("row = " + resMatrix.RowCount);
        Debug.Log("col = " + resMatrix.ColumnCount);
        var inputs = new double[resMatrix.RowCount];
        for(int i = 0; i < resMatrix.RowCount; i++) {
            inputs[i] = resMatrix[i, 0];
        }

        model = VisualStudioLibWrapper.linear_model_create(2);
        // Train Model
        VisualStudioLibWrapper.linear_model_train_classification(model, inputs, 3, 1, Y, 3, 100000, 0.01f);

        double* mdl = (double*)model.ToPointer();
        for(int i = 0; i < 3; i++) {
            Debug.Log("model = " + mdl[i]);
        }

        var c1 = new Color(1, 2, 3);
        var c2 = new Color(3, 2, 1);
        foreach (var testSpheres in testSpheresTransforms)
        {
            double[] input = { testSpheres.position.x, testSpheres.position.z};
            //double y = (float)(-model[1] / model[2] * testSpheresTransform.position.x - model[0] / model[2]);
            float y = (float)VisualStudioLibWrapper.linear_model_predict_classification(model, input, 2);
            // testSpheres.position = new Vector3(
            //     testSpheres.position.x,
            //     y,
            //     testSpheres.position.z
            // );*
            testSpheres.GetComponent<MeshRenderer>().material.color = y >= 0 ? c1 : c2;
        }

        // IntPtr model = VisualStudioLibWrapper.linear_model_create(2);

        // VisualStudioLibWrapper.linear_model_train_classification(model, inputs, 3, 1, Y, 3, 100000, 0.01);


        // var X = new double[trainSpheresTransforms.Length * trainSpheresTransforms.Length *2];
        // var count = 0;
        // for (int i = 0; i < trainSpheresTransforms.Length; i++)
        // {
        //     for (int j = 0; j < trainSpheresTransforms.Length; j++)
        //     {
        //         X[count] = OxMatrix[i, j]; 
        //         //X[count+1] = OzMatrix[i, j];
        //         //count = count+2;
        //         count++;
        //     }
        // }

        // return X;

    }

    /*public unsafe  void TrainAndTest()
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

        //var matrix_inputs = RBFEntries(1, Y);

        // Create Model
        model = VisualStudioLibWrapper.linear_model_create(trainSpheresTransforms.Length);
        // Train Model
        VisualStudioLibWrapper.linear_model_train_classification(model, matrix_inputs, trainSpheresTransforms.Length, 2, Y, Y.Length,1000000, 0.01f);
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
        //VisualStudioLibWrapper.clearArray(model);
    }*/
}
