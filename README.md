# Gesture Recognition in Unity3D

  *Warning*: This is part of a university mini-project, so use at your own risk

Classifies a given action sequence to the class of actions it fits best, and returns the sub-sequence matched as well as the type of action that matched

To train it, the input is a segmented action sequence describing the motion.

The actions are input through action files which have the Cartesian positions of each joint in the skeleton over time. The source of the action files are from Kinect, but the idea can be applied to data taken directly from Kinect.

The algorithm works by applying the dynamic time warping algorithm to find a sub-sequence of the given actions that best matches a set of action sequences acquired through training.

The algorithm was implemented in C# and made as separable as possible from the Unity framework, though not out-of-the-box.

The algorithm needs further improvements for when the actions don't match temporally and spatially, and the definition of a measure (threshold) for gesture matching. Also, more work needs to be done for continuous input.

Though the improvements will be added to a new Matlab implementation coming soon, this code should serve as a good starting point for anyone wishing to implement gesture recognition in Unity3D
