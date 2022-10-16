const express = require('express')
const router = express.Router()
const Doctor= require('../models/doctor.js')

//Getting all
router.get('/', async (req, res) => {
    try{
        const doctors= await Doctor.find()
        res.json(doctors)
    }
    catch(err){
        res.status(500).json({message:err.message})
    }
})

//Getting one
router.get('/:id', getDoctor, (req, res) => {
    res.send(res.doctor.name)
})

//creating one
router.post('/', async(req, res) => {
    const doctor = new Doctor({
        name: req.body.name,
        phoneNumber: req.body.phoneNumber
    })

    try{
        const newDoctor = await doctor.save()
        res.status(201).json(newDoctor)
    }

    catch(err){
        res.status(400).json ({message: err.message})
    }
    }) 

//updating one
router.patch('/:id', getDoctor, async (req, res) => {  
    if(req.body.name != null) {
        res.subscriber.name = req.body.name
    }
    if(req.body.phoneNumber != null) {
        res.doctor.phoneNumber = req.body.phoneNumber
    }
    try{
        const updatedDoctor = await res.doctor.save()
        res.json(updatedDoctor)
    }
    catch(err){
        res.status(400).json({message: err.message})
    }


})

//Deleting one
router.delete('/:id', getDoctor, async(req, res) => {
    try {
        await res.doctor.remove()
        res.json({message:'Deleted Doctor'})
    }

    catch(err) {
        res.status(500).json({message: err.message})
    }
    
})
    async function getDoctor (req, res, next) {
        let doctor
            try{
                subscriber = await Doctor.findById(req.params.id)
                if (doctor ==  null) {
                    return res.status (404).json ({message: 'cannot find doctor'})
                }
            } catch (err) {
                return res.status(500).json ({message: err.message})
            }
            res.doctor = doctor
                next()
    }

module.exports = router