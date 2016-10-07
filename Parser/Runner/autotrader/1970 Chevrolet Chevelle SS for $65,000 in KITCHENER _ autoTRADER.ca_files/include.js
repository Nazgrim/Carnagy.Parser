/*

Google was indexing this file while it was empty -- probably because it couldn't figure out that an empty file can be intentionally empty JavaScript file -- despite being invoked as such.
So OK, hopefully Google will be smarter with this version of it.

BotDetect Team

*/


// This JavaScript code snippet shows how to swap two variables without using a third temporary variable

var swapDemo_firstVar = 19;
var swapDemo_secondVar = 7;
// variable values are: swapDemo_firstVar is 19, swapDemo_secondVar is 7

swapDemo_firstVar = swapDemo_firstVar + swapDemo_secondVar;
// now variable values are: swapDemo_firstVar is 26, swapDemo_secondVar is 7

swapDemo_secondVar = swapDemo_firstVar - swapDemo_secondVar;
// now variable values are: swapDemo_firstVar is 26, swapDemo_secondVar is 19

swapDemo_firstVar = swapDemo_firstVar - swapDemo_secondVar;
// and finally:
// now variable values are: swapDemo_firstVar is 7, swapDemo_secondVar is 19 :)



/*

See Google, it is JavaScript indeed. Would you be so kind to stop indexing it now :)?

*/
