# Machine Learning Navigation — ML Agent vs Rule-Based System

Author: Suhaib Anwaar   

Overview
- This project compares a trained reinforcement learning (ML) agent against a simple rule-based system (RBS) for the task of reaching a target inside a bounded Unity scene.
- The ML agent was trained against the RBS and consistently reaches the target faster than the RBS during evaluation runs. Training ran for multiple hours until the agent reliably reached the target.

Key results
- ML agent reaches the target significantly faster than the RBS 
- The ML agent does not follow a direct straight-line path — it executes circuitous, high-speed maneuvers across the map — but still completes the task in less time than the RBS.
- Reward shaping used during training: positive reward for getting closer to the target, negative reward for moving away, and a bonus reward for reaching the target first.

Rule-Based System (RBS) — behavior summary
- Purpose: provide a deterministic baseline agent that can solve the task using simple heuristics.
- Core behaviors implemented:
  - Move toward the target.
  - Obstacle avoidance using raycasts: rays in front detect walls and trigger avoidance maneuvers.
  - Speed adjustment: when navigating around an obstacle the RBS slows down, and it speeds up again when the path ahead is clear.
- The RBS is reliable but slower due to conservative obstacle negotiation and simpler planning. It uses local sensing and hand-tuned heuristics rather than learned policies.

ML Agent — training & reward design
- Training setup: the agent was trained in the same Unity environment using episodic reinforcement learning, competing indirectly against the RBS as part of training episodes.
- Reward shaping used:
  - Positive incremental reward proportional to reduction in distance to the target.
  - Negative incremental penalty when distance increases.
  - Additional positive reward if the agent reaches the target first in an episode.
- Training terminated after hours of iterative training once the agent reached the target consistently across episodes.

Limitations & future improvements
- Path directness: the ML agent is fast but not path-efficient. Improvements to encourage more direct paths:
  - Add a shaped penalty on unnecessary path length or total travel distance.
  - Add a small per-step time penalty (to bias toward shorter time-to-target).
  - Curriculum training that starts with sparse obstacles and progressively increases complexity.
- Safety & robustness: expand domain randomization (start positions, obstacle layouts) so the agent generalizes to more map variants.
- Additional metrics: add energy or control-effort costs to encourage smoother, more direct motion.
- Extend the RBS baseline with more sophisticated planning (waypoint sequencing, predictive avoidance) to stress-test the ML agent further.
