using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class VisualStudioLibWrapper
{
    [DllImport("ML_project")]
    public static extern IntPtr linear_model_create(int input_dim);
    
    [DllImport("ML_project")]
    public static extern double linear_model_predict_regression(IntPtr model, double[] inputs, int inputs_size);
    
    [DllImport("ML_project")]
    public static extern double linear_model_predict_classification(IntPtr model, double[] inputs, int inputs_size);
    
    [DllImport("ML_project")]
    public static extern void linear_model_train_classification(IntPtr model, double[] dataset_inputs, int dataset_length, int inputs_size, double[] dataset_expected_outputs, int outputs_size, int iterations_count, float alpha);

    [DllImport("ML_project")]
    public static extern void linear_model_train_regression(IntPtr model, double[] dataset_inputs, int dataset_length, int inputs_size, double[] dataset_expected_outputs, int outputs_size, int iterations_count, float alpha);

    [DllImport("ML_project")]
    public static extern void clearArray(IntPtr model);

    [DllImport("ML_project")]
    public static extern IntPtr create_mlp_model(int[] npl, int npl_size);

    [DllImport("ML_project")]
    public static extern IntPtr mlp_model_predict_classification(IntPtr mlp, double[] inputs, bool isReg);
    
    [DllImport("ML_project")]
    public static extern void mlp_model_train_classification(IntPtr mlp, double[] dataset_inputs, int dataset_length, int inputs_size, double[] dataset_expected_outputs, int outputs_size, int epoch, double alpha, bool isReg);
    
    [DllImport("ML_project")]
    public static extern void mlp_model_train_regression(IntPtr mlp, double[] dataset_inputs, int dataset_length, int inputs_size, double[] dataset_expected_outputs, int outputs_size, int epoch, double alpha, bool isReg);

}