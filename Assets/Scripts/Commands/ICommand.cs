using UnityEngine;

public interface ICommand {

	bool Initialize(ICommandArgs args);
	void Stop();
	bool isFinished();
	bool isSuccessful();
}

public interface ICommandArgs {}
