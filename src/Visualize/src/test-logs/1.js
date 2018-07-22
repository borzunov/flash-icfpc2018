import { LogAction } from './LogAction'


const log = []

const makeLog = (pos, type, more) => ({p: pos, t: type, ...more})

log.push(makeLog('0/1/0', LogAction.Add));
for (let i = 0; i < 10; ++i) {
  log.push(makeLog(`${i}/0/0`, LogAction.Fill));
  log.push(makeLog(`${i}/1/0`, LogAction.Remove));
  if (i !== 9) {
    log.push(makeLog(`${i + 1}/1/0`, LogAction.Add));
    log.push(makeLog(`${i + 1}/3/0`, LogAction.FillColor, {c: 'ff0000', o: i / 10}));
  }
}

for (let i = 0; i < 10; ++i) {
  log.push(makeLog(`9/${i}/0`, LogAction.FillColor, {c: '00ff00', o: i / 10}));
}

log.push(makeLog(['3/3/3', '4/4/4', '5/5/5', '6/6/6'], LogAction.FillBatch));
log.push(makeLog(['3/3/2', '4/4/3', '5/5/4', '6/6/5'], LogAction.ColorBatch, {c: '0000ff', o: 0.7}));

export default {
  size: 10,
  log,
  name: 'case1: show features'
}