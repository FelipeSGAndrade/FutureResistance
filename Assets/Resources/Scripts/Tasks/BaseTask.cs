using System;
using System.Collections.Generic;
using UnityEngine;

public class Step {
    public Type commandType;
    public ICommandArgs args;
    public bool done = false;

    public Step(Type type, ICommandArgs args) {
        this.commandType = type;
        this.args = args;
    }
}

public abstract class Task {
    public bool taken = false;
    public TaskType type;

	protected List<Step> stepList = new List<Step>();

    private ICommand currentCommand;
    private Step currentStep;
    private bool done;

    public bool Update(GameObject unit) {
		if (currentCommand == null) {
            currentStep = stepList.Find(step => !step.done);

            if (currentStep == null) {
                done = true;
                return true;
            }

			currentCommand = (ICommand)unit.AddComponent(currentStep.commandType);
            currentCommand.Initialize(currentStep.args);
		}

		if (currentCommand != null && currentCommand.isFinished()) {
			if (!currentCommand.isSuccessful()){
				Abort();
                return false;
			}

            FinishStep();
		}

        return true;
    }
	public void Abort() {
		if (currentCommand != null) {
			currentCommand.Stop();
		}

        FinishStep();
        stepList.ForEach(step => step.done = false);
	}

	public bool IsDone() {
		return done;
	}

	private void FinishStep() {
		UnityEngine.Object.Destroy((MonoBehaviour)currentCommand);
        currentCommand = null;
        currentStep.done = true;
	}
}
