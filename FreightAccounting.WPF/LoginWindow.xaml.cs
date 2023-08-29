﻿using FreightAccounting.Core.Exception;
using FreightAccounting.Core.Interfaces.Repositories;
using FreightAccounting.WPF.Helper;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;

namespace FreightAccounting.WPF;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
    private readonly IUserRepository userRepository;
    private readonly IDebtorRepository debtorRepository;
    private readonly IRemittanceRepository remittanceRepository;
    private readonly IOperatorUserRepository operatorUserRepository;

    public LoginWindow(IUserRepository userRepository ,
        IDebtorRepository debtorRepository ,
        IRemittanceRepository remittanceRepository ,
        IOperatorUserRepository operatorUserRepository)
    {
        InitializeComponent();
        this.userRepository = userRepository;
        this.debtorRepository = debtorRepository;
        this.remittanceRepository = remittanceRepository;
        this.operatorUserRepository = operatorUserRepository;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private async void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var valid = ValidateInputs();


            btnLogin.IsEnabled = false;

            if (valid is not true)
            {
                btnLogin.IsEnabled = true;
                return;
            }

            var userInfo = await userRepository.LoginUser(txtUsername.Text, txtPassword.Password);
            AppSession.LoggedInUsername = userInfo.Username;
            AppSession.LoggedInUserId = userInfo.Id;

            new MainWindow(debtorRepository,remittanceRepository,operatorUserRepository).ShowDialog();
        }
        catch (AppException ax)
        {
            MessageBox.Show(ax.Message);
        }
        catch (Exception ex)
        {
            MessageBox.Show("در عملیات ثبت خطایی رخ داده");
            Logger.LogException(ex);
        }
    }

    private bool ValidateInputs()
    {
        if(string.IsNullOrEmpty(txtPassword.Password) || string.IsNullOrEmpty(txtUsername.Text))
        {
            MessageBox.Show("نام کاربری و کلمه عبور نمیتواند خالی باشد");
            return false;
        }
        return true;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {

    }
}
