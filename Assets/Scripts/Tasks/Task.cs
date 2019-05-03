using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task {
    protected TaskType type;
    public TaskType Type { get { return type; } }

    protected TaskStatus status = TaskStatus.CREATED;
    public TaskStatus Status { get { return status; } }

    private Step currentStep;
    private ICommand currentCommand;
	protected List<Step> stepList = new List<Step>();

    private bool initialized;
    private bool done;

    public abstract TaskStatus Validate();
    public abstract bool Initialize();

    public override string ToString() {
        return "Type: " + Type;
    }

    public static implicit operator bool(Task exists) {
        return exists != null;
    }

    public Task(TaskType type) {
        this.type = type;
    }

    public bool Update(GameObject unit) {
        if (!initialized) {
            initialized = Initialize();
        }

        if (status != TaskStatus.READY) {
            return false;
        }

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
