using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Assets.Scripts.Helpers {
    
    public class StateMachine {
        private Dictionary<string, State> States = new Dictionary<string, State>();

        public State CurrentState { get; private set; }
        public bool DebugMode { get; set; } = false;

        public State CreateState(string name) {
            var state = new State {
                Name = name.ToLower()
            };
            AddState(state);
            return state;
        }

        public void AddState(State state) {
            state.Name = state.Name.ToLower();
            States.Add(state.Name, state);
        }

        public void Start() {
            if (States.Count == 0) {
                Debug.Log("No states.");
            } else {
                var s = States.ElementAt(0).Value;
                TransitionTo(s);
            }
        }

        public void Update() {
            CurrentState?.OnStay?.Invoke();
        }

        public void TransitionTo(State state) {
            Exit(CurrentState);
            Enter(state);
        }

        public void TransitionTo(string name) {
            if (States.TryGetValue(name.ToLower(), out State state)) {
                TransitionTo(state);
            }
        }

        public State GetState(string name) {
            return States.TryGetValue(name.ToLower(), out var s) ? s : null;
        }


        private void Enter(State s) {
            if (DebugMode) Debug.Log($"Entering {s}");
            s?.OnEnter?.Invoke();
            CurrentState = s;
        }

        private void Exit(State s) {
            if (DebugMode) Debug.Log($"Exiting {s}");
            s?.OnExit?.Invoke();
        }

        public class State {
            public string Name;
            public Action OnEnter;
            public Action OnExit;
            public Action OnStay;

            public override string ToString() => Name;
        }

    }
}