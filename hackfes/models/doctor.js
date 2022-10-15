const mongoose= require ('mongoose')
const doctorSchema = new mongoose.Schema({

    name:{
        type: String,
        required: true
    },

    phoneNumber:{
        type: Number,
        required: true
    },

    })

module.exports = mongoose.model('Doctor', doctorSchema)