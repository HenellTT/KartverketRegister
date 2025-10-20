const prompts = [
	"Write a quirky one-sentence 'Are you sure?' message for a HARD RESET. " +
	"End it with a keyword wrapped in *asterisks* (like *YES* or *PURGE*, *etc*). " +
	"Tell the user to type that keyword to confirm.",
	"Give me an unusual 'Are you sure' message, one sentence long, its to make sure user wants to HARD RESET database, end it with a phase type 'true, yes, accept, positive, etc', Keep the phase in * instead of quotes! and tell userto type it!"

]
let msgin = null;
function setMsginToAi() {
	if (msgin == null) {
		try {
			msgin = await ApiFreeLLM.chat(prompts[0]);
		} catch {
			setMsginToAi();
		}
	}
}

async function resetdb() {
	if (msgin == null) {
		alert("Loading Confirmation message, Click OK to wait in peace");
		try {
			msgin = await ApiFreeLLM.chat(prompts[0]);
		} catch {
			msgin = "Are you sure you want to delete and reset the WHOLE database? Type *yes* to continue...";
		}
	}
	



	let rightpin = findKeyWord(msgin);
	console.log(rightpin);
	let pin = prompt(msgin);
	console.log(msgin);
	console.log(pin);


	if (pin.toLowerCase() == rightpin.toLowerCase()) {
		fetch("/admin/ResetDB");
		alert("Ok db resetted!");
	} else {
		alert("Ok, nah no reset today!");
	}

}
function findKeyWord(aiFeedback) {
	split = aiFeedback.split("*").map((words) => { return words.split(" ") }).filter((a) => a.length == 1 && a[0].length >= 3);
	return split[split.length - 1][0];
}

