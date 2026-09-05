# Harden Tycho’s per-module transactional consistency

## Summary

Tycho has the correct foundation: a scoped transaction and a shared TychoDbContext allow business changes, outgoing events, and inbox completion to commit atomically. The main weaknesses are transaction lifecycle management, failure handling, claim ownership, and notification timing.

Preserve module independence. As agreed, include registered request interceptors in transactional execution, reject missing transaction providers and unmanaged transactions, and explicitly reject EF retrying execution strategies for now.

Analysis and baseline validation are complete: 40 EF persistence tests and 360 framework unit tests passed using their executable runners. No source files were changed. Existing tests do not cover the failure sequences below.

## Findings and targeted corrections

Cost includes implementation and meaningful regression tests: S = localized change; M = coordinated changes; L = provider-sensitive recovery work.

Order    Issue                                                        Cost    Why this position
━━━━━━━  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  ━━━━━━  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    1    Roll back when inbox acknowledgement fails                     -     Highest immediate consistency gain: prevents a worker that lost ownership from committing business changes.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    2    Notify inbox processors only after commit                      -     Straightforward reuse of the existing mechanism; eliminates premature wake-ups and potentially long processing delays.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    3    Serialize the entire outbox batch before tracking entries      -     Very small change that prevents partial publication when serialization fails and the caller catches the exception.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    4    Isolate notification failures from persistence outcomes        -     Prevents successfully committed operations being reported as failures and retried unnecessarily. Cover both deferred and immediate notifications.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    5    Repair transaction lifecycle and callback ownership           M      Foundational fix: remove stale transaction references, validate transitions, clear callbacks, and make disposal consistent. Include rejection of overlapping begins.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    6    Make cancellation and failure cleanup reliable                M      Ensures rollback is attempted despite cancellation and preserves the original exception. Builds on lifecycle cleanup.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    7    Prevent reuse of rolled-back tracked state                    M      Prevents failed work leaking into subsequent saves. Pair tracker cleanup with fresh scopes for failure bookkeeping.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    8    Include registered interceptors in the transaction           S–M     Broadens atomicity to the complete request pipeline. Small code change, but a deliberate behavioral change requiring broader regression coverage.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    9    Reject retrying EF execution strategies explicitly             -     Cheap guard against an unsupported execution mode before business work starts.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    10    Reject transactional execution through EmptyTransaction       -     Makes missing guarantees explicit. Requires checking affected examples and tests for intentional in-memory usage.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    11    Reject unmanaged external transactions                        M      Prevents false commit assumptions and premature notifications. Requires consistent ownership checks across transaction and writer paths.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    12    Reject conflicting or partial persistence registrations       M      Prevents silent context mismatches or durable/in-memory mixing; mainly protects configuration edge cases.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    13    Make destination inbox receipt idempotent                    M–L     Significant reliability gain after delivery/acknowledgement failures, but concurrent insertion and error classification make this more expensive.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    14    Distinguish uncertain commit outcomes                         L      Important failure semantics, but needs careful recovery behavior and fault injection. Explicitly reporting uncertainty is achievable; resolving it reliably is a larger concern.
───────  ───────────────────────────────────────────────────────────  ──────  ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    15    Normalize exhausted expired claims and expose diagnostics    S–M     Improves operational recovery and truthful state reporting; does not directly strengthen business-data atomicity.

Two supporting items should accompany the relevant fixes:

- Guarantee documentation: update alongside changes—module/context boundaries, standalone publishing behavior, isolation limits, and unsupported configurations.
- Provider concurrency tests: add with claim fencing and deduplication. Treat claim acquisition behavior across providers as something to verify, rather than an already-proven defect.

Items 5–7 should be implemented consecutively: together they establish dependable transaction failure handling. The rest can largely remain separate, reviewable changes.

## Implementation decisions and guarantee boundaries

- Transaction contract: retain the internal ITransaction abstraction and public transactional handler interfaces. Represent active, committed, rolled-back, faulted/unknown, and disposed states explicitly. No database operation may follow an invalid lifecycle transition. Both request and inbox execution must follow the same cleanup and outcome rules.
- Commit uncertainty: distinguish failure before commit from an exception during commit whose durable outcome is unknown. Do not claim successful rollback proves that an uncertain commit never happened. Expose a specific commit-outcome exception to request callers; inbox recovery checks persisted state in a fresh scope before conditionally marking the original claim
failed. EF documents both uncertain commits and the incompatibility with implicitly retrying execution strategies. EF connection resiliency
- Retry policy: reject a retrying EF execution strategy before invoking a transactional handler. Do not automatically replay request handlers. Inbox retries continue in fresh scopes. Reject multiple invocations of a transactional pipeline’s continuation so retry interceptors cannot reuse a contaminated unit of work.
- Inbox deduplication: check both tracked and persisted entries for matching ID and immutable message content. At the isolated delivery boundary, recover a concurrent insertion failure only after the failed scope is disposed and a fresh read confirms the identical destination entry exists. A conflicting message with the same ID remains an error.
- Claim timing: retain conditional acknowledgement by claim ID and state as the commit fence. Expiration makes a claim eligible for replacement; it does not by itself prove replacement occurred. A timeout is cooperative and cannot stop arbitrary handler code, so correctness must depend on acknowledgement ownership, not timeout assumptions.
- Exhausted claims: the current consumers exclude expired final-attempt claims forever while leaving them InProcessing. Normalize these to the existing Failed state with exhausted attempts and emit diagnostics. Preserve bounded retries; make exhaustion visible rather than implying eventual successful delivery.
- Configuration: make EmptyTransaction.BeginAsync fail clearly for transactional handlers. Reject multiple different persistence-context registrations within one module, and reject partially configured persistence services that would silently mix durable and in-memory components.
- Atomicity boundary: guarantees cover transactional handlers and writes through the registered module context. Unmarked handlers, other contexts, external services, and in-memory queues do not acquire atomicity. Standalone publishing currently saves all pending changes on that context; document this behavior explicitly.
- Isolation: keep the provider’s default isolation level. Atomic commit does not prevent every lost update or write-skew anomaly. Document application concurrency tokens, conditional updates, and stronger isolation where business invariants require them. Do not claim universal serializability or exactly-once external effects.

These changes need no messaging-table migration. The intentional compatibility changes are pipeline ordering, lifecycle validation, and explicit rejection of unsupported configurations.

## Validation and delivery

Add regression tests that verify durable database state through separate contexts, rather than only mock call ordering:
- Lost or replaced inbox claim: business changes and outgoing events both roll back.
- Handler failure after an explicit SaveChangesAsync: neither business changes nor acknowledgement persists.
- Cancelled operation plus rollback/disposal failure: cleanup is attempted and the original error remains identifiable.
- Successful commit followed by another transaction; repeated commit; rollback followed by attempted reuse; synchronous and asynchronous disposal.
- Throwing notification subscriber: committed request still succeeds, remaining notifications run, and polling can recover missed wake-ups.
- Inbox notification occurs only after commit; rolled-back writes produce none.
- Interceptor writes before and after the handler commit together; an interceptor exception rolls them all back.
- Duplicate and concurrent destination deliveries converge on one inbox entry; unrelated insert failures and conflicting content remain errors.
- Mid-batch serialization failure leaves no partially staged outbox.
- Missing provider, unmanaged transaction, retrying execution strategy, and conflicting registrations fail before business work.
- Simulated commit acknowledgement loss is reported as uncertain and does not trigger blind replay.
- Expired final-attempt claims become visibly exhausted.

Keep SQLite regression coverage, and add SQL Server and PostgreSQL integration coverage for competing claims, transaction failure, and concurrent deduplication. The current single-provider tests cannot establish those concurrency guarantees.

Deliver lifecycle/cleanup and claim fencing first, followed by pipeline/notification changes, then deduplication and configuration enforcement. Add structured diagnostics for unknown commits, cleanup failures, lost claims, notification failures, and exhausted attempts.
