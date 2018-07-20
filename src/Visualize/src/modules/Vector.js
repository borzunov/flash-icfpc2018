const pool = {}

class VectorImpl {
  constructor(x, y, z) {
    this.x = x
    this.y = y
    this.z = z
    Object.freeze(this)
  }

  serialize() {
    return VectorImpl.getKey(this.x, this.y, this.z)
  }

  static getKey(x, y, z) {
    return `${x}/${y}/${z}`
  }
}

export const getVectorKey = VectorImpl.getKey;

function Vector (x, y, z) {
  if (new.target) {
    throw new Error('just call Vector without new');
  }
  let key = VectorImpl.getKey(x, y, z)
  pool[key] = pool[key] || new VectorImpl(x, y, z);
  return pool[key];
}

export default Vector;