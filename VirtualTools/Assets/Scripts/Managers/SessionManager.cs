using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager
{
    private static SessionManager instance;

    public SessionManager Instance()
    {
        if (instance == null)
            instance = new SessionManager();

        return instance;
    }
}
