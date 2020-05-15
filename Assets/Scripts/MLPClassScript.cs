
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MLPClassScript : MonoBehaviour
{
    public Transform[] trainSpheresTransforms;

    public Transform[] testSpheresTransforms;
    
    public int[] npl;

    public double[] classSphere;
    public unsafe void TrainAndTest()
    {
        Debug.Log("Training and Testing");

        var Y = new double[trainSpheresTransforms.Length];

        var inputs = new double[trainSpheresTransforms.Length *2];

        for (int i = 0; i < trainSpheresTransforms.Length; i++)
        {
            /*if(trainSpheresTransforms[i].position.y == classSphere[2]) {
                Y[i] = classSphere[2];
            } else if (trainSpheresTransforms[i].position.y == classSphere[1] ) {
                Y[i] = classSphere[1];
            } else if (trainSpheresTransforms[i].position.y == classSphere[0] ) {
                Y[i] = classSphere[0];
            }*/
            Y[i] = trainSpheresTransforms[i].position.y >= 0 ? 1 : -1;
            inputs[i*2] = trainSpheresTransforms[i].position.x;
            inputs[(i*2)+1] = trainSpheresTransforms[i].position.z;
        }

        //int[] npl = {2, 1};
        IntPtr model = VisualStudioLibWrapper.create_mlp_model(npl, npl.Length);
        VisualStudioLibWrapper.mlp_model_train_classification(model, inputs, trainSpheresTransforms.Length, 2, Y, Y.Length, 1000000, 0.001f);

        // For each testSphere : Predict 
        double max = -2;
        int idmax = 1;
        
        foreach (var testSpheres in testSpheresTransforms)
        {
            double[] input = { testSpheres.position.x, testSpheres.position.z};
            IntPtr y = VisualStudioLibWrapper.mlp_model_predict_classification(model, input, false);
            /*double[] res = new double[npl[npl.Length - 1]];
            Marshal.Copy(res, 0, y, res.Length);*/
            double* r = (double*) y.ToPointer();

            // Debug.Log("size = " + npl[npl.Length - 1]);
            //Debug.Log(.GetLength(0));
            max = r[1];
            for(int i = 1; i < classSphere.Length+1; i++) {
                //Debug.Log("r =" + r[i]);
                if(r[i] > max) {
                    max = r[i];
                    idmax = i;
                }
            }
            if(classSphere.Length == 2){
                testSpheres.position = new Vector3(
                    testSpheres.position.x,
                    (float)r[1],
                    testSpheres.position.z
                );
            }else{
                testSpheres.position = new Vector3(
                    testSpheres.position.x,
                    (float)classSphere[idmax-1],
                    testSpheres.position.z
                );
            }
        }

        VisualStudioLibWrapper.clearArray(model);
    }
}
