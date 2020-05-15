﻿
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

    public double getRBFValue(double epsilon, Transform x1, Transform x2)
    {
        //return Math.Exp(- epsilon *  Math.Pow(x1 - x2, 2.0));
        //Debug.Log("X1(" + x1.position.x + " " + x1.position.y + " " + x1.position.z + "), X2(" + x2.position.x + " " + x2.position.y + " " + x2.position.z + ")");
        /*var res = Math.Exp((-1) * epsilon *  Math.Pow(Math.Pow(
            Math.Pow(x2.position.x - x1.position.x, 2.0) + Math.Pow(x2.position.z - x1.position.z, 2.0), 
            0.5), 
            2));
        //Debug.Log("RES:" + res);
        return res;*/

        double x = Math.Pow(x1.position.x - x2.position.x, 2);
        double y = Math.Pow(x1.position.y - x2.position.y, 2);
        double z = Math.Pow(x1.position.z - x2.position.z, 2);
        double gam = -1 * epsilon * (x + y + z);
        return Math.Exp(gam);
    }

    public double getRBFValueD(double epsilon, double x1, double x2)
    {
        double x = Math.Pow(x1 - x2, 2);
        double normalized = Math.Sqrt(x);
        double pow = Math.Pow(normalized, 2);
        double gam = -1 * epsilon * pow;
        return Math.Exp(gam);
        /*var res = Math.Exp((-1) * epsilon *  Math.Pow(
                    Math.Pow(
                        Math.Pow(x1-x2,2)
                    ,0.5)
                , 2.0));
        //Debug.Log("RES:" + res);
        return res;*/
    }


    public double[] RBFEntries(double epsilon, double[] Y)
    {

        var Ox = new double[trainSpheresTransforms.Length,trainSpheresTransforms.Length];
        //var Oz = new double[trainSpheresTransforms.Length,trainSpheresTransforms.Length];
        var multiY = new double[Y.Length, 1];

        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            for (int j = 0; j < trainSpheresTransforms.Length; j++)
            {
                Ox[i,j] = getRBFValue(epsilon, trainSpheresTransforms[i], trainSpheresTransforms[j]);
                //Oz[i,j] = getRBFValue(epsilon, trainSpheresTransforms[i].position.z, trainSpheresTransforms[j].position.z);
            }
            multiY[i,0] = Y[i];
        }

        Matrix<double> OxMatrix = DenseMatrix.OfArray(Ox);
        //Matrix<double> OzMatrix = DenseMatrix.OfArray(Oz);
        Matrix<double> YMatrix = DenseMatrix.OfArray(multiY);
        //Debug.Log(OxMatrix.RowCount);
        //Debug.Log(OxMatrix.ColumnCount);

        Matrix<double> OxInv = OxMatrix.Inverse();
        //Matrix<double> OzInv = OzMatrix.Inverse();

        Matrix<double> Wx = OxInv.Multiply(YMatrix);

        
        var X = new double[trainSpheresTransforms.Length];
        var count = 0;
        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
                X[count] = Wx[i, 0]; 
                //X[count+1] = OzMatrix[i, j];
                //count = count+2;
                Debug.Log("final : " + Wx[i, 0]);
                count++;
        }

        return X;

    }

    
    public double[] RBFEntriesD(double epsilon, double[] Y)
    {

        var Ox = new double[trainSpheresTransforms.Length,trainSpheresTransforms.Length];
        var Oz = new double[trainSpheresTransforms.Length,trainSpheresTransforms.Length];
        var multiY = new double[Y.Length, 1];

        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            for (int j = 0; j < trainSpheresTransforms.Length; j++)
            {
                Ox[i,j] = getRBFValueD(epsilon, trainSpheresTransforms[i].position.x, trainSpheresTransforms[j].position.x);
                Oz[i,j] = getRBFValueD(epsilon, trainSpheresTransforms[i].position.z, trainSpheresTransforms[j].position.z);
            }
            multiY[i,0] = Y[i];

        }
        Debug.Log(multiY.ToString());

        Matrix<double> OxMatrix = DenseMatrix.OfArray(Ox);
        Matrix<double> OzMatrix = DenseMatrix.OfArray(Oz);
        Matrix<double> YMatrix = DenseMatrix.OfArray(multiY);

        Matrix<double> OxInv = OxMatrix.Inverse();
        Matrix<double> OzInv = OzMatrix.Inverse();

        Matrix<double> Wx = OxInv.Multiply(YMatrix);
        Matrix<double> Wz = OzInv.Multiply(YMatrix);

        
        var X = new double[trainSpheresTransforms.Length *2];
        var count = 0;
        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            X[count*2] = Wx[i, 0]; 
            X[(count*2)+1] = Wz[i, 0];
            Debug.Log("final : " + Wx[i, 0] + " , " +Wz[i, 0]);
            Debug.Log("inv : " + OxInv[i, 0] + " , " +OzInv[i, 0]);
            Debug.Log("base : " + OxMatrix[i, 0] + " , " +OzMatrix[i, 0]);
        }

        return X;

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

        var matrix_inputs = RBFEntriesD(0.5, Y);

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
    }
}
