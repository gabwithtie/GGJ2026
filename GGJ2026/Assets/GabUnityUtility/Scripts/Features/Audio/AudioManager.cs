using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
	public class AudioManager : Manager_Base<AudioManager>
	{
		struct SoundPlayedRecently
		{
			public AudioClip clip;
			public float age;
		}

		private List<SoundPlayedRecently> sounds_played_recently = new();
		[SerializeField] private float soundThreshold = 0.08f; // Example threshold for sound age

		private void Update()
		{
			for (int i = 0; i < sounds_played_recently.Count; i++)
			{
				var currecent = sounds_played_recently[i];
				currecent.age += Time.deltaTime; // Increment the age of the sound

				sounds_played_recently[i] = currecent; // Update the list with the incremented age

				if (sounds_played_recently[i].age > soundThreshold) // Example threshold of 5 seconds
				{
					sounds_played_recently.RemoveAt(i);
					i--;
				}
			}
		}

		public static bool CheckPlayedRecently(AudioClip clip)
		{
			foreach (var sound in Instance.sounds_played_recently)
			{
				if (sound.clip == clip && sound.age < Instance.soundThreshold)
				{
					return true; // Sound has been played recently
				}
			}

			return false;
		}

		public static void RegisterPlayedAudio(AudioClip clip)
		{
			SoundPlayedRecently newSound = new SoundPlayedRecently
			{
				clip = clip,
				age = 0f
			};
			Instance.sounds_played_recently.Add(newSound);
		}
	}
}