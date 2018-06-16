/// <summary>
/// Aids in statuses having multiple factors. For example, invincibility might be affected by post-player-damage,
/// an ability activation, powerups, etc. so each ability can have its own timer.
/// </summary>
public class StatusTimers {
	public delegate void TimerStatusUpdated();
	public event TimerStatusUpdated OnTimerOff;
	public event TimerStatusUpdated OnTimerOn;

	private float[] timers = new float[8];
	private bool isOn;  // Whether all timers are <= 0

	/// <summary>
	/// Adds a new timer
	/// </summary>
	/// <returns>The index of the new timer</returns>
	/// <param name="time">Time to initialize the timer to.</param>
	public int Add(float time) {
		if (time > 0 && !isOn) {
			isOn = true;
			if (OnTimerOn != null)
				OnTimerOn();
		}
		for (int i = 0; i < timers.Length; i++) {
			if (timers[i] <= 0f) {
				timers[i] = time;
				return i;
			}
		}
		// If there are no empty timer slots, double the array size and copy the old array over
		int oldLength = timers.Length;
		float[] newArr = new float[oldLength * 2];
		timers.CopyTo(newArr, 0);
		// Add the timer value to the new, bigger array
		timers = newArr;
		timers[oldLength] = time;
		return oldLength;
	}

	/// <summary>
	/// Decrements each timer.
	/// </summary>
	/// <param name="amt">Amount to decrement by.</param>
	public void DecrementTimer(float amt) {
		bool foundTimer = false;
		for (int i = timers.Length - 1; i >= 0; i--) {
			if (timers[i] >= 0) {
				timers[i] -= amt;
				foundTimer = true;
				// Turn timer on
				if (!isOn) {
					if (OnTimerOn != null)
						OnTimerOn();
					isOn = true;
				}
			}
		}
		// Turn timer off
		if (!foundTimer && isOn) {
			if (OnTimerOff != null)
				OnTimerOff();
			isOn = false;
		}
	}

	public void RemoveTimer(int i) {
		timers[i] = 0;
	}

	public bool IsOn() {
		//for (int i = 0; i < timers.Length; i ++) {
		//	if (timers[i] > 0)
		//		return true;
		//}
		//return false;
		return isOn;
	}
}