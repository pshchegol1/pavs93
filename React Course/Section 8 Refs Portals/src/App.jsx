import Player from './components/Player.jsx';
import TimeChallenge from './components/TimerChallenge.jsx';

function App() {
  return (
    <>
      <Player />
      <div id="challenges">
        <TimeChallenge title='EASY' targetTime={1}/>
        <TimeChallenge title='NOT EASY' targetTime={5}/>
        <TimeChallenge title='GETTING TOUGH' targetTime={10}/>
        <TimeChallenge title='PROS ONLY' targetTime={15}/>
      </div>
    </>
  );
}

export default App;
