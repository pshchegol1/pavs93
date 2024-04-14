import { useState, useRef } from "react";

export default function Player() {

//*REF*
const playerName = useRef();

const [enteredPlayerName, setEnteredPlayerName] = useState("");


function handleClick() {
  //*SET REF*/
  setEnteredPlayerName(playerName.current.value);
  playerName.current.value = '';
  //setSubmitted(true);
}

  return (
    <section id="player">
      <h2>Welcome {enteredPlayerName ?? 'unknown entity'}</h2>
      <p>
        <input ref={playerName} type="text" />
        <button onClick={handleClick}>Set Name</button>
      </p>
    </section>
  );
}
