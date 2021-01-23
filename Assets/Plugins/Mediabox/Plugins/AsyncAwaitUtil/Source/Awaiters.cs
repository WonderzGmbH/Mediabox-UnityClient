using System;
using UnityEngine;

// TODO: Remove the allocs here, use a static memory pool?
public static class Awaiters {
	static readonly WaitForUpdate _waitForUpdate = new WaitForUpdate();
	static readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
	static readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

	public static WaitForUpdate NextFrame { get { return _waitForUpdate; } }

	public static WaitForFixedUpdate FixedUpdate { get { return _waitForFixedUpdate; } }

	public static WaitForEndOsfFrame EndOfFrame { get { return _waitForEndOfFrame; } }

	public static WaitForSeconds Seconds(float seconds) {
		return new WaitForSeconds(seconds);
	}

	public static WaitForSecondsRealtime SecondsRealtime(float seconds) {
		return new WaitForSecondsRealtime(seconds);
	}

	public static WaitUntil Until(Func<bool> predicate) {
		return new WaitUntil(predicate);
	}

	public static WaitWhile While(Func<bool> predicate) {
		return new WaitWhile(predicate);
	}
}