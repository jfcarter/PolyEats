using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User{

    public string email;
    public string password;
    public string firstName;
    public string lastName;
    //TO DO: Picture
    //TO DO: Payment

	public User(string email, string password, string firstName, string lastName)
    {
        this.email = email;
        this.password = password;
        this.firstName = firstName;
        this.lastName = lastName;
    }

    public string getEmail()
    {
        return email;
    }

    public string getFirstName()
    {
        return firstName;
    }

    public string getLastName()
    {
        return lastName;
    }
}
