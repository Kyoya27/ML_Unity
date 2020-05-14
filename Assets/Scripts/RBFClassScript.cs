
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
    struct Projection
    {
        double W;
        double[] X;
    }

    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;

    private int transfo = -1;

    /*void   addCenterPoint(const double y, const std::vector<double>& x);
    void   computeWeights(const bool useRegularization = false, const double lambda = 0.1);
    double getInterpolatedValue(const std::vector<double>& x) const;
    */
    public Vector3 AppliTransfo(Vector3 position)
    {
        var posX = position.x;
        var posZ = position.z;

        //SOFT
        if(transfo == 0){
            if ((posX <= 5 && posX >= 4 && posZ >= 3 && posZ <= 4) || (posX >= -3 && posX <= -2 && posZ >= 9 && posZ <= 10))
            {
                posX += 5;
                posZ += 5;
            }

            if (posX <= 4 && posX >= 3 && posZ >= 6 && posZ <= 7)
            {
                posX -= 5;
                posZ -= 5;
            }
        }

        //CROSS
        if(transfo == 1){
            posX = Math.Abs(position.x);
            posZ = Math.Abs(position.z);
            if (posX > 3 && posZ > 3 )
            {
                posX += 3f;
                posZ += 3f;
            }
        }

        //XOR
        if(transfo == 2){
            posX = (float)Math.Pow(position.x + position.z, 2);
            posZ = (float)Math.Pow(position.x + position.z, 2);
        }
        return new Vector3(posX, position.y, posZ);
    }


    public void TransfoSoftThenTrain()
    {
        transfo = 0;
    }

    public void TransfoCrossThenTrain()
    {
        transfo = 1;
    }

    public void TransfoXORThenTrain()
    {
        transfo = 2;
    }

    public double getRBFValue(double epsilon, double x1, double x2)
    {
        return Math.exp(- epsilon *  Math.pow(Math.Abs(x1 - x2), 2.0));
    }


    public double[] RBFEntries(double epsilon, double[] Y)
    {

        var Ox = new double[trainSpheresTransforms.Length,trainSpheresTransforms.Length];
        var Oz = new double[trainSpheresTransforms.Length,trainSpheresTransforms.Length];

        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            for (int j = 0; j < trainSpheresTransforms.Length; j++)
            {
                Ox[i,j] = getRBFValue(epsilon, trainSpheresTransforms[i].position.x, trainSpheresTransforms[j].position.x);
                Oz[i,j] = getRBFValue(epsilon, trainSpheresTransforms[i].position.z, trainSpheresTransforms[j].position.z);
            }
        }

        Matrix<double> OxMatrix = DenseMatrix.OfArray(Ox);
        Matrix<double> OzMatrix = DenseMatrix.OfArray(Oz);
        Matrix<double> YMatrix = DenseMatrix.OfArray(Y);

        Matrix<double> OxInv = OxMatrix.Inverse();
        Matrix<double> OzInv = OzMatrix.Inverse();



        return 

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
            var position = new Vector3();
            if(transfo != -1)
                position = AppliTransfo(trainSpheresTransforms[i].position);
            else
                position = trainSpheresTransforms[i].position;
            Y[i] = position.y > 0 ? 1 : -1;
            linear_inputs[i*2] = position.x;
            linear_inputs[(i*2)+1] = position.z;
        }

        var matrix_inputs = RBFEntries(1, Y);

        // Create Model
        model = VisualStudioLibWrapper.linear_model_create(trainSpheresTransforms.Length);
        // Train Model
        VisualStudioLibWrapper.linear_model_train_classification(model, linear_inputs, trainSpheresTransforms.Length, 2, Y, Y.Length,1000000, 0.01f);
        //double* model = {-0.4, 0.4, 0.2};
        //double[] model = new double [] {-0.4, 0.4, 0.2};

        // For each testSphere : Predict 
        foreach (var testSpheres in testSpheresTransforms)
        {
            var position = new Vector3();
            if(transfo != -1)
                position = AppliTransfo(testSpheres.position);
            else
                position = testSpheres.position;
            double[]input = { position.x, position.z};
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
