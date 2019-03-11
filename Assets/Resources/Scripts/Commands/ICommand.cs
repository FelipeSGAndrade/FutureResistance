
public interface ICommand {

	void Execute(ICommandArgs args);
	void Stop();
	bool isFinished();
	bool isSuccessful();
}

public interface ICommandArgs {}
