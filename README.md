
# Project 1

By: Marcus Elosegui and Shreyans Jain

## Running the Programs

The miner file can be found in the SHAGenerator folder. To run it use the command:
	
	dotnet fsi actor.fsx numZeros

The miner will produce a lot of warnings in red, but these have no effect on the program 
and it will still mine the coins.

The server files can be found in the Remote_Test2 folder. You must first run actor_local.fsx
with the command:

	dotnet fsi actor_local.fsx numZeros

Then you can run the remote actor file with:

	dotnet fsi actor_remote.fsx


## Report

We decided that the size of our work units would be rather large to keep the number of actors to a minimum. Preventing a slow down
due to a constant stream of communication was something we sought to avoid. As a result we only had two actors, the miner and the boss. 


The boss was responsible for spawning all of the miner actors and shutting down the program when the coins have been found. The miner 
actor has a large workload. They created the input strings, hashed the strings, checked for the leading zeros, and sent out a message to
the boss indicating if a coin was found. We determined that this would be the best implementation because these computations are relatively
simple and handled by F# libraries rather quickly. This would be much more efficient than spawning many different actors responsible for 
each part of that workload and then having them communicate with each other constantly.

The program was able to successfully mine dozens of coins with an input of 4 leading zeroes. The running time for was measured with 
"#time "on"" and the result of running it was: 
	
	Real: 00:00:15.728, CPU: 00:01:30.359 -> Ratio: 5.75
This was ran on a machine with 6 cores.

The coin with the most leading zeros we found was 7:

	Real: 00:08:32.590, CPU: 00:44:41.125

We were not able to get a successful server running in time, so the largest number of working machines we used was 1.
