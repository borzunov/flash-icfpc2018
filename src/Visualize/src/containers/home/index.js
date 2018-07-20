//import { push } from 'connected-react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import ThreeScene from './ThreeScene'
import { changeSize, changeVoxel } from '../../modules/space'

const mapStateToProps = ({ space }) => ({
  size: space.size,
})

const mapDispatchToProps = dispatch =>
  bindActionCreators(
    {
      changeSize,
      changeVoxel,
    },
    dispatch
  )

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ThreeScene)
