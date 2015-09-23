#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Date    : 2015-09-13 15:33:42
# @Author  : n1k0 (nikobin222@gmail.com)
# @Link    : http://n1k0.top

import os
import sys 
import subprocess

line = 0 

allpath = ['G:\\code\\android\\CAFETOOLS\\app\\src\\main\\java',\
			'G:\\code\\windows\\K3yManager',\
			'G:\\code\\android\\AManager\\app\\src\\main\\java']

def getfile(path) :
	for root ,dirname ,filename in os.walk(path):
		for f in filename:
			if checkfile(f):
				readfile(path+"\\"+f)
		for dirnext in dirname:
			if dirnext =='obj' or dirnext=='Properties':
				continue
			getfile(path+"\\"+dirnext)	

def checkfile(f):
	ftag = os.path.splitext(f)[1] 
	if ftag == '.java' or ftag =='.xml' or ftag=='.cs' \
	or ftag=='.xaml' or ftag==".py":
		return True 


def  readfile(path) :
	global line 
	
	if os.path.exists(path):
		f=open(path) ; 
		for  i in f.read() :
			if i =='\n':
				line+=1
		f.close() 

def  main():
	oldlines =[]
	global line ,allpath 
	f=open('G:\\lines.txt','r+')
	for i in f.readlines() :
		oldlines.append(i.replace('\n',''))  
	print  oldlines 
	for i,path in enumerate(allpath) :
		getfile(path)
		print '+++++++++++++++++++++++++++++++++++++++++++'
		print '[*] ',path,'lines:' , line 
		print '[*] new lines:'  , line-int(oldlines[i])
		print '+++++++++++++++++++++++++++++++++++++++++++'
		oldlines[i] =str(line)+'\n'   
		line = 0 
	print oldlines 
	f.seek(0,0)
	f.write(''.join(oldlines))
	f.close()	
	print "[*] Exiting....."

if __name__ == '__main__':
	main()

