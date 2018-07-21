import React from 'react'

export default function HelpText() {
  return <div style={{opacity: 0.7, textAlign: 'right', position: 'absolute', right: 20, top: 20, color: 'white'}}>
    <div>Open in chrome and install <a target="_blank" rel="noopener noreferrer"
                                       href="https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd?hl=en">redux
      dev tools</a></div>
    <div>Coordinates: <span style={{ color: 'red' }}>X </span><span style={{ color: 'green' }}>Y </span><span
      style={{ color: 'blue' }}>Z </span> <span>!!!Z is inverted in regard to task</span></div>
    <div>Left mouse - rotate camera</div>
    <div>Mouse scroll - zoom</div>
    <div>Right mouse - pan</div>
    <div>Arrow buttons - move camera</div>
  </div>
}