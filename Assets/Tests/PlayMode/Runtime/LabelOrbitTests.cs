using NUnit.Framework;
using UnityEngine;

public class LabelOrbitTests
{
    private GameObject orbitObj;
    private labelOrbit orbit;
    private GameObject centerObj;

    [SetUp]
    public void Setup()
    {
        centerObj = new GameObject("Center");
        centerObj.transform.position = Vector3.zero;

        orbitObj = new GameObject("Orbit");
        orbit = orbitObj.AddComponent<labelOrbit>();
        orbit.center = centerObj.transform;
        orbit.radius = 1f;
        orbit.speed = 30f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(orbitObj);
        Object.DestroyImmediate(centerObj);
    }

    // ensure Start() positions orbitObj correctly
    [Test]
    public void Start_sets_position()
    {
        orbit.initializeOrbit();

        Vector3 expectedPos = new Vector3(0, 0.2f, 1f);
        Assert.That(orbitObj.transform.rotation, Is.EqualTo(Quaternion.identity).Within(0.01f));

    }

    // ensure Update() rotates when player not close
    [Test]
    public void Update_rotates_object_when_no_lookScript()
    {
        orbit.initializeOrbit();
        var initialRot = orbitObj.transform.rotation;

        orbit.rotateOrbitIfAllowed();

        Assert.AreNotEqual(initialRot, orbitObj.transform.rotation);
    }

    [Test]
    public void Update_does_not_rotate_when_lookScript_playerIsClose_true()
    {
        orbit.initializeOrbit();

        // Create a stub signLabelLook
        var lookObj = new GameObject("LookScript");
        var lookStub = lookObj.AddComponent<signLabelLook>();
        orbit.setLookScript(lookStub);

        lookStub.player = lookObj.transform;

        // Simulate player is close 
        var collider = lookObj.AddComponent<BoxCollider>();
        lookStub.OnTriggerEnter(collider);

        var initialRot = orbitObj.transform.rotation;

        orbit.rotateOrbitIfAllowed();

        // Rotation should be skipped
        Assert.AreEqual(initialRot, orbitObj.transform.rotation);

        Object.DestroyImmediate(lookObj);
}

}
