﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class PatrolBetweenPoints : MonoBehaviour {

	// Punkterna definieras genom varje undersåte till /pointsParent/
	public Transform pointsParent;

	// Enheter per sekund
	public float speed = 5;

	// true=börjar om från början vid slutet
	// false=går baklänges
	public bool repeat = false;

	// Går den nu baklänges genom punkterna?
	public bool backwards = false;

	// Vilken distans räknas som att man är framme?
	private const float stoppingDistance = 0.1f;

	private Transform target;

	private void GetNearest(List<Transform> all) {
		
	}

	private void NextTarget() {
		if (pointsParent == null) {
			// Avbryt funktionen
			target = null;
			return;
		}

		List<Transform> allChildren = new List<Transform>();
		Transform nearest = null;
		float dist = 0;

		foreach(Transform child in pointsParent) {
			allChildren.Add(child);

			float thisDist = Vector3.Distance(child.position, transform.position);
			if (!nearest || thisDist < dist) {
				nearest = child;
				dist = thisDist;
			}
		}

		if (allChildren.Count == 0) {
			// Inga children, inga target points
			target = null;
		} else {
			// Hitta nästa
			int index = allChildren.IndexOf(target);

			if (index == -1) {
				// Ta närmsta
				target = nearest;
			} else {
				// Ta nästa
				if (backwards) {
					index--;
					if (!repeat && index < 0) {
						backwards = false;
						index = 1;
					}
				} else {
					index++;
					if (!repeat && index >= allChildren.Count) {
						backwards = true;
						index = allChildren.Count - 2;
					}
				}

				index += allChildren.Count;
				index %= allChildren.Count;
				target = allChildren[index];
			}
		}
	}

	private void Update() {
		// Jämför avståndet
		if (target == null || Vector3.Distance(target.position, transform.position) < stoppingDistance) {
			NextTarget();
		}

		if (target != null) {
			Object comp;

			if (comp = GetComponent<NavMeshAgent>()) {
				// Försök styra via NavMesh pathfinding
				NavMeshAgent agent = comp as NavMeshAgent;

				agent.speed = speed;

				// Välj inte ny väg om redan räknar ut en
				if (!agent.pathPending) {
					if (!agent.hasPath) {
						// Sätt nästa punkt som mål
						agent.SetDestination(target.position);
					} else if (agent.isStopped) {
						// Välj nästa mål och sätt som mål
						NextTarget();
						agent.SetDestination(target.position);
					}
				}
			} else if (comp = GetComponent<Rigidbody>()) {
				// Försök styra via Rigidbody
				Rigidbody body = comp as Rigidbody;

				// Räkna ut riktigten mot nästa punkt
				Vector3 direction = target.position - transform.position;
				direction.y = 0;
				direction.Normalize();

				// Ändra objektets hastighet
				Vector3 velocity = body.velocity + direction * speed;
				// Ignorera y axeln
				velocity.y = 0;
				// Lås till maxHastigheten
				velocity = Vector3.ClampMagnitude(velocity, speed);
				// Håll kvar gravitationen
				velocity.y = body.velocity.y;
				// Applisera
				body.velocity = velocity;

			} else if (comp = GetComponent<CharacterController>()) {
				// Försök styra via CharacterController
				CharacterController controller = comp as CharacterController;

				// Räkna ut riktigten mot nästa punkt
				Vector3 direction = target.position - transform.position;
				direction.y = 0;
				direction.Normalize();

				// Förflytta sig
				controller.SimpleMove(direction * speed);

			} else if (comp = GetComponent<Rigidbody2D>()) {
				// Försök styra via RigidBody2D
				Rigidbody2D body = comp as Rigidbody2D;

				// Räkna ut riktigten mot nästa punkt
				Vector2 velocity = body.velocity;
				if (target.position.x > transform.position.x) {
					// Förflytta sig åt höger
					velocity += Vector2.right * speed;
				} else {
					// Förflytta sig åt vänster
					velocity += Vector2.left * speed;
				}
				// Lås till maxHastigheten
				velocity = Vector3.ClampMagnitude(velocity, speed);
				// Håll kvar gravitationen
				velocity.y = body.velocity.y;
				// Applisera
				body.velocity = velocity;

			} else {
				transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
			}
		}
	}

	// Visualisering av vägen
	private void OnDrawGizmos() {

		NavMeshAgent agent = GetComponent<NavMeshAgent>();

		if (pointsParent != null) {

			Transform first = null;
			Transform last = null;

			// Gå igenom alla
			foreach (Transform child in pointsParent) {
				if (first == null) {
					first = child;
				}

				if (last != null) {
					// Rita linje mellan förra och denna
					Gizmos.color = new Color(1, 0, 1, 0.5f);
					Gizmos.DrawLine(last.position, child.position);
					// Rita sfär vid förra
					Gizmos.color = new Color(0, 0, 1, 0.8f);
					Gizmos.DrawWireSphere(last.position, stoppingDistance);
				}

				last = child;
			}

			if (last) {
				// Rita sfär vid sista
				Gizmos.color = new Color(0, 0, 1, 0.8f);
				Gizmos.DrawWireSphere(last.position, stoppingDistance);

				if (first && repeat) {
					// Rita linje mellan första och sista
					Gizmos.color = new Color(1, 0, 1, 0.5f);
					Gizmos.DrawLine(last.position, first.position);
				}
			}
		}

		if (target == null) {
			NextTarget();
		}

		if (target && (!agent || !agent.hasPath)) {
			// Rita linje mellan sig själv och nästa punkt
			Gizmos.color = new Color(0, 1, 0.2f, 0.8f);
			Gizmos.DrawLine(transform.position, target.position);

			if (pointsParent != null) {
				// Rita sfär vid målet
				Gizmos.color = new Color(0, 0, 1, 0.8f);
				Gizmos.DrawWireSphere(target.position, stoppingDistance);
			}
		}

		if (!Application.isPlaying) {
			target = null;
		}

		if (agent && agent.hasPath) {
			Vector3[] corners = agent.path.corners;

			// Ritar linjer mellan alla hörn längst pathfinding vägen
			for (int i=0; i<corners.Length-1; i++) {
				Gizmos.color = new Color(0, 1, 0.2f, 0.8f);
				Gizmos.DrawLine(corners[i], corners[i + 1]);

				// Rita sfär vid första
				if (i == 0) {
					Gizmos.color = new Color(0, 0, 1, 0.8f);
					Gizmos.DrawWireSphere(corners[0], stoppingDistance);
				}
				// Rita sfär vid alla förutom första
				Gizmos.color = new Color(0, 0, 1, 0.8f);
				Gizmos.DrawWireSphere(corners[i + 1], stoppingDistance);
			}
		}
	}

}