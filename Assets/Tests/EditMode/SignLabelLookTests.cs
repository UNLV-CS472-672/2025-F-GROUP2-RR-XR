using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SignLabelLookTests
{
    private GameObject obj;
    private signLabelLook script;
    private GameObject playerObj;

    [SetUp]
    public void Setup()
    {
        obj = new GameObject("Label");
        script = obj.AddComponent<signLabelLook>();

        playerObj = new GameObject("Player");
        playerObj.transform.position = Vector3.zero;

        script.player = playerObj.transform;
        script.radius = 2f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(playerObj);
    }

    // ensure getPlayerIsClose initializes false
    [Test]
    public void getPlayerIsClose_initially_false()
    {
        Assert.IsFalse(script.getPlayerIsClose());
    }

    // ensure getPlayerIsClose is true upon entering the trigger
    [Test]
    public void OnTriggerEnter_sets_playerIsClose_true()
    {
        var collider = playerObj.AddComponent<BoxCollider>();
        script.OnTriggerEnter(collider);

        Assert.IsTrue(script.getPlayerIsClose());
    }
    
    // ensure getPlayerIsClose is false upon exiting the trigger
    [Test]
    public void OnTriggerExit_sets_playerIsClose_false()
    {
        var collider = playerObj.AddComponent<BoxCollider>();
        script.OnTriggerEnter(collider); // make it true first
        script.OnTriggerExit(collider);

        Assert.IsFalse(script.getPlayerIsClose());
    }

    // ensure Update rotates toward player when close
    [Test]
    public void Update_rotates_toward_player_when_close()
    {
        script.OnTriggerEnter(playerObj.GetComponent<Collider>());

        obj.transform.rotation = Quaternion.identity;
        var initialRot = obj.transform.rotation;

        script.rotateTowardPlayer();

        Assert.AreNotEqual(initialRot, obj.transform.rotation);
    }
}
